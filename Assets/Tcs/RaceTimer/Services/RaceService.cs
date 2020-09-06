using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Tcs.RaceTimer.ViewModels;
using Tcs.RaceTimer.Exceptions;
using Dawn;
using System.Net.Mail;

namespace Tcs.RaceTimer.Services
{
    public class RaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly RaceCategoryRepository _raceCategoryRepository;
        private readonly RacePlayerTimeRepository _racePlayerTimeRepository;
        private readonly RacePlayerRepository _racePlayerRepository;

        private readonly BehaviorSubject<Race> _currentRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<RaceCategory> _currentRaceCategory = new BehaviorSubject<RaceCategory>(null);

        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<Player> _newPlayer = new BehaviorSubject<Player>(null);
        private readonly BehaviorSubject<Team> _newTeam = new BehaviorSubject<Team>(null);
        private readonly BehaviorSubject<Category> _newCategory = new BehaviorSubject<Category>(null);

        private readonly BehaviorSubject<RaceCategoryViewModel> _newRaceCategory = new BehaviorSubject<RaceCategoryViewModel>(null);
        private readonly BehaviorSubject<RacePlayerViewModel> _newRacePlayer = new BehaviorSubject<RacePlayerViewModel>(null);

        private readonly Subject<string> _raceCategoryDeleted = new Subject<string>();

        public Race CurrentRace { get; private set; }
        public RaceCategory CurrentRaceCategory { get; private set; }

        public IObservable<Race> OnRaceLoaded() => _currentRace.AsObservable();
        public IObservable<RaceCategory> OnRaceCategoryLoaded() => _currentRaceCategory.AsObservable();

        public IObservable<Race> OnNewRace() => _newRace.AsObservable();
        public IObservable<Player> OnNewPlayer() => _newPlayer.AsObservable();
        public IObservable<Team> OnNewTeam() => _newTeam.AsObservable();
        public IObservable<Category> OnNewCategory() => _newCategory.AsObservable();
        public IObservable<RacePlayerViewModel> OnNewRacePlayer() => _newRacePlayer.AsObservable();
        public IObservable<RacePlayerViewModel> OnNewRaceCategoryPlayer() => _newRacePlayer
            .Where(x => x != null && CurrentRaceCategory != null && x.Category.Id == CurrentRaceCategory.CategoryId).AsObservable();
        public IObservable<RaceCategoryViewModel> OnNewRaceCategory() => _newRaceCategory.AsObservable();
        public IObservable<string> OnRaceCategoryDeleted() => _raceCategoryDeleted.AsObservable();

        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository,
            CategoryRepository categoryRepository,
            RaceCategoryRepository raceCategoryRepository,
            RacePlayerTimeRepository racePlayerTimeRepository,
            RacePlayerRepository racePlayerRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _categoryRepository = categoryRepository;
            _raceCategoryRepository = raceCategoryRepository;
            _racePlayerTimeRepository = racePlayerTimeRepository;
            _racePlayerRepository = racePlayerRepository;
        }

        public void LoadRace(string raceId)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();

            CurrentRace = _raceRepository.Get(raceId);
            _currentRace.OnNext(CurrentRace);

            // Load first category
            var raceCategories = _raceCategoryRepository.GetAll(raceId);
            if (raceCategories != null)
            {
                var firstCategory = raceCategories.First();
                LoadRaceCategory(firstCategory.Id);
            }
            else
            {
                _currentRaceCategory.OnNext(null);
            }
        }

        public void LoadRaceCategory(string raceCategoryId)
        {
            Guard.Argument(raceCategoryId, nameof(raceCategoryId))
                .NotNull().NotEmpty().NotWhiteSpace();

            CurrentRaceCategory = _raceCategoryRepository.Get(raceCategoryId);
            _currentRaceCategory.OnNext(CurrentRaceCategory);
        }

        public void SetAutoTiming(
            TimeSpan startTime,
            int playerIntervalSeconds,
            int categoryIntervalSeconds,
            int stageIntervalSeconds,
            bool hasBreak,
            TimeSpan? startTimeAfterBreak,
            int? stageAfterBreak)
        {
            Guard.Argument(playerIntervalSeconds, nameof(playerIntervalSeconds))
                .NotZero().NotNegative();
            Guard.Argument(categoryIntervalSeconds, nameof(categoryIntervalSeconds))
                .NotZero().NotNegative();
            Guard.Argument(stageIntervalSeconds, nameof(stageIntervalSeconds))
                .NotZero().NotNegative();

            TimeSpan date = startTime;
            TimeSpan previousStageStartTime;
            int stage = 1;

            do
            {
                if (hasBreak)
                {
                    if (!stageAfterBreak.HasValue || !startTimeAfterBreak.HasValue)
                        throw new AutoTimingException("No start time and stage after break set when the add break flag is true");

                    if (stage == stageAfterBreak.Value)
                    {
                        date = startTimeAfterBreak.Value;
                    }
                }

                previousStageStartTime = date;

                var raceCategories = GetAllRaceCategories();
                foreach (var raceCategory in raceCategories)
                {
                    var raceCategoryPlayers = GetAllRaceCategoryPlayers(raceCategory.Category.Id);
                    foreach (var raceCategoryPlayer in raceCategoryPlayers)
                    {
                        var player = _playerRepository.Get(raceCategoryPlayer.Player.Id);

                        var logTime = new LogTime
                        {
                            Hours = date.Hours,
                            Minutes = date.Minutes,
                            Seconds = date.Seconds,
                            Milliseconds = date.Milliseconds
                        };

                        var result = CreateRacePlayerTime(
                            CurrentRace.Id,
                            player.Id,
                            raceCategory.Category.Id,
                            stage,
                            logTime,
                            TimeType.Start);

                        if (result == null)
                            throw new AutoTimingException("Failed to create race player time");

                        date = date.Add(new TimeSpan(0, 0, playerIntervalSeconds));
                    }

                    // Subtract last added player interval time
                    date = date.Subtract(new TimeSpan(0, 0, playerIntervalSeconds));

                    date = date.Add(new TimeSpan(0, 0, categoryIntervalSeconds));
                }

                date = previousStageStartTime.Add(new TimeSpan(0, 0, stageIntervalSeconds));

            } while (++stage <= CurrentRace.Stages);

            // Refresh current race category
            _currentRaceCategory.OnNext(CurrentRaceCategory);
        }

        public Race CreateRace(string name, long eventDate, int stages, string location)
        {
            var id = $"Race-{Guid.NewGuid()}";
            var newRace = CreateRace(id, name, eventDate, stages, location);
            return newRace;
        }

        public Race CreateRace(string id, string name, long eventDate, int stages, string location)
        {
            Guard.Argument(id, nameof(id))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(name, nameof(name))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(eventDate, nameof(eventDate))
                .NotZero().NotNegative();
            Guard.Argument(stages, nameof(stages))
                .NotZero().NotNegative();
            
            var newRace = _raceRepository.Create(
                new Race
                {
                    Id = id,
                    Name = name,
                    EventDate = eventDate,
                    Stages = stages,
                    Location = location
                });
            _newRace.OnNext(newRace);
            return newRace;
        }

        public RacePlayerTime CreateRacePlayerTime(string raceId, string playerId, string categoryId, int stage, LogTime logTime, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(playerId, nameof(playerId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .NotZero().NotNegative();

            var newId = Guid.NewGuid().ToString();
            var racePlayerTime = new RacePlayerTime
            {
                Id = newId,
                RaceId = raceId,
                PlayerId = playerId,
                CategoryId = categoryId,
                Stage = stage,
                Time = logTime,
                Type = timeType
            };

            var result = _racePlayerTimeRepository.CreateOrUpdate(racePlayerTime);
            return result;
        }

        public Team CreateTeam(string name)
        {
            var newId = $"Team-{Guid.NewGuid()}";
            return CreateTeam(newId, name);
        }

        public Team CreateTeam(string id, string name)
        {
            Guard.Argument(id, nameof(id))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(name, nameof(name))
                .NotNull().NotEmpty().NotWhiteSpace();

            var team = _teamRepository.Create(
                new Team
                {
                    Id = id,
                    Name = name
                });
            _newTeam.OnNext(team);
            return team;
        }

        public Player CreatePlayer(string name, int age, string email)
        {
            var newId = _playerRepository.GenerateId();
            return CreatePlayer(newId, name, age, email);
        }

        public Player CreatePlayer(string id, string name, int age, string email)
        {
            Guard.Argument(id, nameof(id))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(name, nameof(name))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(age, nameof(age))
                .NotZero().NotNegative();
            Guard.Argument(email, nameof(email))
                .NotNull().NotEmpty().NotWhiteSpace();

            var player = _playerRepository.Create(
                new Player
                {
                    Id = id,
                    Name = name,
                    Age = age,
                    Email = email
                });
            _newPlayer.OnNext(player);
            return player;
        }

        public RacePlayer CreateRacePlayer(string name, int age, string email, string categoryName, string teamName)
        {
            var racePlayerId = Guid.NewGuid().ToString();
            return CreateRacePlayer(racePlayerId, name, age, email, categoryName, teamName);
        }

        public RacePlayer CreateRacePlayer(string id, string name, int age, string email, string categoryName, string teamName)
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            Guard.Argument(id, nameof(id))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(name, nameof(name))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(age, nameof(age))
                .NotZero().NotNegative();
            Guard.Argument(email, nameof(email))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(categoryName, nameof(categoryName))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(teamName, nameof(teamName))
                .NotNull().NotEmpty().NotWhiteSpace();

            var player = _playerRepository.FindByName(name);
            if (player == null)
            {
                player = CreatePlayer(name, age, email);
            }

            if (string.IsNullOrEmpty(teamName) || string.IsNullOrWhiteSpace(teamName))
                return null;

            var team = _teamRepository.FindByName(teamName);
            if (team == null)
            {
                var teamId = Guid.NewGuid().ToString();
                team = CreateTeam(teamId, teamName);
            }

            if (string.IsNullOrEmpty(categoryName) || string.IsNullOrWhiteSpace(categoryName))
                return null;

            var category = _categoryRepository.FindByName(categoryName);
            if (category == null)
            {
                var categoryId = Guid.NewGuid().ToString();
                category = CreateCategory(categoryId, categoryName);
            }

            if (player != null && team != null && category != null)
            {
                var racePlayer = _racePlayerRepository.Find(CurrentRace.Id, team.Id, player.Id);
                if (racePlayer == null)
                {
                    racePlayer = _racePlayerRepository.Create(
                        CurrentRace.Id,
                        new RacePlayer
                        {
                            Id = id,
                            RaceId = CurrentRace.Id,
                            CategoryId = category.Id,
                            TeamId = team.Id,
                            PlayerId = player.Id
                        });
                }

                _newRacePlayer.OnNext(
                    new RacePlayerViewModel
                    {
                        Id = racePlayer.Id,
                        Race = CurrentRace,
                        Team = team,
                        Category = category,
                        Player = player
                    });

                return racePlayer;
            }

            return null;
        }

        public Category CreateCategory(string name)
        {
            var newId = $"Category-{Guid.NewGuid()}";
            return CreateCategory(newId, name);
        }

        public Category CreateCategory(string id, string name)
        {
            Guard.Argument(id, nameof(id))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(name, nameof(name))
                .NotNull().NotEmpty().NotWhiteSpace();

            var category = _categoryRepository.Create(new Category
            {
                Id = id,
                Name = name
            });
            _newCategory.OnNext(category);
            return category;
        }

        public RaceCategory AddRaceCategory(string categoryName)
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            Guard.Argument(categoryName, nameof(categoryName))
                .NotNull().NotEmpty().NotWhiteSpace();

            var race = _raceRepository.Get(CurrentRace.Id);
            var category = _categoryRepository.FindByName(categoryName);
            if (category == null)
            {
                category = CreateCategory(categoryName);
            }

            // Don't add it if it's already been added
            var exists = _raceCategoryRepository.FindByCategory(race.Id, category.Id);
            if (exists != null)
                throw new CategoryAlreadyAddedToRaceException();

            var newId = Guid.NewGuid().ToString();
            var raceCategory = _raceCategoryRepository.Create(race.Id, new RaceCategory
            {
                Id = newId,
                CategoryId = category.Id,
                RaceId = race.Id
            });

            _newRaceCategory.OnNext(
                new RaceCategoryViewModel
                {
                    Id = newId,
                    Race = race,
                    Category = category
                });

            // Load first category if there is only one category in the list
            var raceCategories = _raceCategoryRepository.GetAll(CurrentRace.Id);
            if (raceCategories.Count() == 1)
            {
                var firstCategory = raceCategories.First();
                LoadRaceCategory(firstCategory.Id);
            }

            return raceCategory;
        }

        public Category GetCategory(string categoryId)
        {
            return _categoryRepository.Get(categoryId);
        }

        public IEnumerable<Race> GetAllRaces()
        {
            return _raceRepository.GetAll();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _playerRepository.GetAll();
        }

        public IEnumerable<RaceCategoryViewModel> GetAllRaceCategories()
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            var raceCategories = _raceCategoryRepository.GetAll(CurrentRace.Id);
            var result = raceCategories
                .Select(x => new RaceCategoryViewModel
                {
                    Id = x.Id,
                    Race = _raceRepository.Get(x.RaceId),
                    Category = _categoryRepository.Get(x.CategoryId)
                });
            return result;
        }

        public IEnumerable<RacePlayerViewModel> GetAllRaceCategoryPlayers(string categoryId)
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace();

            var raceCategoryPlayers = _racePlayerRepository.GetAll(CurrentRace.Id).Where(x => x.CategoryId == categoryId);
            var result = raceCategoryPlayers
                .Select(x => new RacePlayerViewModel
                {
                    Id = x.Id,
                    Category = _categoryRepository.Get(x.CategoryId),
                    Player = _playerRepository.Get(x.PlayerId),
                    Team = _teamRepository.Get(x.TeamId),
                    Race = _raceRepository.Get(x.RaceId),
                    PlayerTimes = _racePlayerTimeRepository.GetAllByRaceCategoryPlayer(CurrentRace.Id, x.CategoryId, x.PlayerId).ToList()
                });

            return result;
        }

        public IEnumerable<RacePlayerViewModel> GetAllRacePlayers()
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            var racePlayers = _racePlayerRepository.GetAll(CurrentRace.Id);
            
            return racePlayers.Select(rp => 
                new RacePlayerViewModel
                {
                    Id = rp.Id,
                    Race = CurrentRace,
                    Category = _categoryRepository.Get(rp.CategoryId),
                    Team = _teamRepository.Get(rp.TeamId),
                    Player = _playerRepository.Get(rp.PlayerId),
                    PlayerTimes = _racePlayerTimeRepository.GetAllByRaceCategoryPlayer(CurrentRace.Id, rp.CategoryId, rp.PlayerId).ToList()
                });
        }

        public IEnumerable<RacePlayerViewModel> GetAllRaceCategoryPlayers()
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            if (CurrentRaceCategory == null)
                throw new RaceCategoryNotLoadedException();

            return GetAllRaceCategoryPlayers(CurrentRaceCategory.CategoryId);
        }

        public void DeleteRaceCategory(RaceCategory raceCategory)
        {
            if (raceCategory == null)
                throw new ArgumentNullException(nameof(raceCategory));

            var existingRaceCategory = _raceCategoryRepository.Get(raceCategory.Id);
            if (existingRaceCategory != null)
            {
                // Delete race category players first
                DeleteRaceCategoryPlayers(raceCategory.RaceId, raceCategory.CategoryId);

                // Delete race category altogether
                _raceCategoryRepository.Delete(raceCategory.RaceId, raceCategory.Id);
                _raceCategoryDeleted.OnNext(raceCategory.Id);
            }
        }

        public void DeleteRaceCategoryPlayers(string raceId, string categoryId)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace();

            var raceCategoryPlayers = _racePlayerRepository.GetAll(CurrentRace.Id).Where(x => x.CategoryId == categoryId);
            _racePlayerRepository.Delete(raceId, raceCategoryPlayers.Select(x => x.Id));
        }

        public void DeleteRacePlayer(string raceId, string categoryId, string teamId, string playerId)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(teamId, nameof(teamId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(playerId, nameof(playerId))
                .NotNull().NotEmpty().NotWhiteSpace();

            var racePlayer = _racePlayerRepository.Find(raceId, teamId, categoryId, playerId);
            if (racePlayer != null)
            {
                _racePlayerRepository.Delete(raceId, racePlayer.Id);
            }
        }
    }
}

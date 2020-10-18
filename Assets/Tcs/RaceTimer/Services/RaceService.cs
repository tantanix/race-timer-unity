using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Tcs.RaceTimer.ViewModels;
using Tcs.RaceTimer.Exceptions;
using Dawn;
using Tcs.RaceTimer.Enums;

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

        private readonly Subject<Race> _currentRace = new Subject<Race>();
        private readonly Subject<RaceCategory> _currentRaceCategory = new Subject<RaceCategory>();
        private readonly Subject<int> _currentStage = new Subject<int>();

        private readonly Subject<Race> _newRace = new Subject<Race>();
        private readonly Subject<Player> _newPlayer = new Subject<Player>();
        private readonly Subject<Team> _newTeam = new Subject<Team>();
        private readonly Subject<Category> _newCategory = new Subject<Category>();
        private readonly Subject<RaceCategoryViewModel> _newRaceCategory = new Subject<RaceCategoryViewModel>();
        private readonly Subject<RacePlayerViewModel> _newRacePlayer = new Subject<RacePlayerViewModel>();
        private readonly Subject<RacePlayerTimeViewModel> _newRacePlayerTime = new Subject<RacePlayerTimeViewModel>();

        private readonly Subject<RacePlayerTimeViewModel> _racePlayerTimeUpdated = new Subject<RacePlayerTimeViewModel>();

        private readonly Subject<string> _racePlayerTimeDeleted = new Subject<string>();
        private readonly Subject<string> _raceCategoryDeleted = new Subject<string>();

        public Race CurrentRace { get; private set; }
        public RaceCategory CurrentRaceCategory { get; private set; }

        public int CurrentStage { get; private set; }

        public IObservable<Race> OnRaceLoaded() => _currentRace.AsObservable();
        public IObservable<RaceCategory> OnRaceCategoryLoaded() => _currentRaceCategory.AsObservable();
        public IObservable<int> OnStageSet() => _currentStage.AsObservable();

        public IObservable<Race> OnNewRace() => _newRace.AsObservable();
        public IObservable<Player> OnNewPlayer() => _newPlayer.AsObservable();
        public IObservable<Team> OnNewTeam() => _newTeam.AsObservable();
        public IObservable<Category> OnNewCategory() => _newCategory.AsObservable();
        public IObservable<RacePlayerTimeViewModel> OnNewRacePlayerTime() => _newRacePlayerTime.AsObservable();
        public IObservable<RacePlayerViewModel> OnNewRacePlayer() => _newRacePlayer.AsObservable();
        public IObservable<RacePlayerViewModel> OnNewRaceCategoryPlayer() => _newRacePlayer
            .Where(x => x != null && CurrentRaceCategory != null && x.Category.Id == CurrentRaceCategory.CategoryId).AsObservable();
        public IObservable<RaceCategoryViewModel> OnNewRaceCategory() => _newRaceCategory.AsObservable();
        public IObservable<string> OnRaceCategoryDeleted() => _raceCategoryDeleted.AsObservable();
        public IObservable<string> OnRacePlayerTimeDeleted() => _racePlayerTimeDeleted.AsObservable();

        public IObservable<RacePlayerTimeViewModel> OnRacePlayerTimeUpdated() => _racePlayerTimeUpdated.AsObservable();

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
            if (raceCategories != null && raceCategories.Any())
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

        public void SetStage(int stage)
        {
            CurrentStage = stage;
            _currentStage.OnNext(stage);
        }

        public void SetAutoNumbering(int startCount)
        {
            Guard.Argument(startCount, nameof(startCount))
                .NotZero().NotNegative();

            int playerNo = 0;

            var raceCategories = GetAllRaceCategories();
            foreach (var raceCategory in raceCategories)
            {
                var raceCategoryPlayers = GetAllRaceCategoryPlayers(raceCategory.Category.Id);
                foreach (var raceCategoryPlayer in raceCategoryPlayers)
                {
                    UpdateRacePlayerNo(raceCategoryPlayer.Id, ++playerNo);
                }
            }
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
                        var logTime = new LogTime
                        {
                            Hours = date.Hours,
                            Minutes = date.Minutes,
                            Seconds = date.Seconds,
                            Milliseconds = date.Milliseconds
                        };

                        var result = CreateRacePlayerTime(
                            CurrentRace.Id,
                            raceCategoryPlayer.Id,
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

            // Emit race category changes
            _currentRaceCategory.OnNext(CurrentRaceCategory);
        }

        public Race CreateRace(string name, long eventDate, int stages, string location)
        {
            var id = _raceRepository.GenerateId();
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

        public RacePlayerTime CreateRacePlayerTime(string raceId, string racePlayerId, string categoryId, int stage, LogTime logTime, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(racePlayerId, nameof(racePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .NotZero().NotNegative();

            var newId = _racePlayerTimeRepository.GenerateId();
            var racePlayerTime = new RacePlayerTime
            {
                Id = newId,
                RaceId = raceId,
                RacePlayerId = racePlayerId,
                CategoryId = categoryId,
                Stage = stage,
                Time = logTime,
                Type = timeType
            };

            var result = _racePlayerTimeRepository.CreateOrUpdate(racePlayerTime);
            return result;
        }

        public RacePlayerTime CreateRacePlayerTime(string raceId, int stage, LogTime logTime, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .NotZero().NotNegative();

            var newId = _racePlayerTimeRepository.GenerateId();
            var racePlayerTime = new RacePlayerTime
            {
                Id = newId,
                RaceId = raceId,
                Stage = stage,
                Time = logTime,
                Type = timeType
            };

            var result = _racePlayerTimeRepository.Create(raceId, racePlayerTime);

            _newRacePlayerTime.OnNext(new RacePlayerTimeViewModel
            {
                Id = result.Id,
                Time = result.Time,
                PlayerNo = !string.IsNullOrEmpty(result.RacePlayerId) ? _racePlayerRepository.Get(result.RacePlayerId)?.No : null
            });

            return result;
        }

        public Team CreateTeam(string name)
        {
            var newId = _teamRepository.GenerateId();
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
            var racePlayerId = _racePlayerRepository.GenerateId();
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
                var teamId = _teamRepository.GenerateId();
                team = CreateTeam(teamId, teamName);
            }

            if (string.IsNullOrEmpty(categoryName) || string.IsNullOrWhiteSpace(categoryName))
                return null;

            var category = _categoryRepository.FindByName(categoryName);
            if (category == null)
            {
                var categoryId = _categoryRepository.GenerateId();
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
            var newId = _categoryRepository.GenerateId();
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

            var newId = _raceCategoryRepository.GenerateId();
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

        public IEnumerable<RacePlayerTimeViewModel> GetRacePlayerLogTimes(string raceId, int stage, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .NotZero().NotNegative();

            return _racePlayerTimeRepository
                .GetAllByRaceAndStage(raceId, stage, timeType)
                .Select(rpt => new RacePlayerTimeViewModel
                {
                    Id = rpt.Id,
                    Time = rpt.Time,
                    PlayerNo = !string.IsNullOrEmpty(rpt.RacePlayerId) ? _racePlayerRepository.Get(rpt.RacePlayerId)?.No : null,
                    Status = rpt.Status,
                });
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
                    No = x.No,
                    PlayerTimes = _racePlayerTimeRepository.GetAllByRaceCategoryPlayer(CurrentRace.Id, x.CategoryId, x.Id).ToList()
                });

            return result;
        }

        public RacePlayer GetRacePlayer(string raceId, string playerId)
        {
            Guard.Argument(playerId, nameof(playerId))
                .NotNull().NotEmpty().NotWhiteSpace();

            return _racePlayerRepository.Get(playerId);
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

        public void UpdatePlayerNoOfFirstUnassignedRacePlayerTime(string raceId, int stage, string playerNo)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .GreaterThan(0).LessThan(6);
            Guard.Argument(playerNo, nameof(playerNo))
                .NotNull().NotEmpty();

            var unassignedRacePlayerTimes = GetAllUnassignedRacePlayerTimes(raceId, stage);
            if (unassignedRacePlayerTimes.Any())
            {
                var firstUnassignedRacePlayerTime = unassignedRacePlayerTimes.FirstOrDefault();
                if (firstUnassignedRacePlayerTime != null)
                {
                    playerNo = playerNo.Trim();
                    if (int.TryParse(playerNo, out var riderNo))
                    {
                        UpdateRacePlayerTimePlayerNo(firstUnassignedRacePlayerTime.Id, riderNo);
                    }
                }
            }
        }

        public IEnumerable<RacePlayerTime> GetAllUnassignedRacePlayerTimes(string raceId, int stage)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(stage, nameof(stage))
                .GreaterThan(0).LessThan(6);
            
            var unassignedRacePlayerTimes = _racePlayerTimeRepository
                .GetAllUnassignedByRaceAndStage(raceId, stage, TimeType.End);

            return unassignedRacePlayerTimes;
        }

        public void UpdateRacePlayerNo(string racePlayerId, int playerNo)
        {
            Guard.Argument(racePlayerId, nameof(racePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(playerNo, nameof(playerNo))
                .GreaterThan(0);

            var racePlayer = _racePlayerRepository.Get(racePlayerId);
            if (racePlayer != null)
            {
                racePlayer.No = playerNo;
                _racePlayerRepository.Update(racePlayer);
            }
        }

        public void UpdateRacePlayerTimePlayerNo(string racePlayerTimeId, string playerNo)
        {
            Guard.Argument(playerNo, nameof(playerNo))
                .NotNull().NotEmpty();

            playerNo = playerNo.Trim();
            if (int.TryParse(playerNo, out var riderNo))
            {
                UpdateRacePlayerTimePlayerNo(racePlayerTimeId, riderNo);
            }
            else
            {
                // Invalid player no input
                var racePlayerTime = _racePlayerTimeRepository.Get(racePlayerTimeId);
                racePlayerTime.Status = PlayerLogTimeStatus.InvalidPlayerNo;
                _racePlayerTimeRepository.Update(racePlayerTime);

                _racePlayerTimeUpdated.OnNext(new RacePlayerTimeViewModel
                {
                    Id = racePlayerTime.Id,
                    Status = racePlayerTime.Status,
                });
            }
        }

        public void UpdateRacePlayerTimePlayerNo(string racePlayerTimeId, int playerNo)
        {
            Guard.Argument(racePlayerTimeId, nameof(racePlayerTimeId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(playerNo, nameof(playerNo))
                .GreaterThan(0);

            var racePlayerTime = _racePlayerTimeRepository.Get(racePlayerTimeId);
            var racePlayer = _racePlayerRepository.Find(racePlayerTime.RaceId, playerNo);
            if (racePlayer != null)
            {
                var racePlayerAlreadyAssigned = _racePlayerTimeRepository
                    .GetByRaceAndStageAndPlayerNo(racePlayerTime.RaceId, racePlayerTime.Stage, racePlayer.Id, TimeType.End) != null;

                if (!racePlayerAlreadyAssigned)
                {
                    // Valid
                    racePlayerTime.RacePlayerId = racePlayer.Id;
                    racePlayerTime.Status = PlayerLogTimeStatus.Valid;
                    _racePlayerTimeRepository.Update(racePlayerTime);

                    _racePlayerTimeUpdated.OnNext(new RacePlayerTimeViewModel
                    {
                        Id = racePlayerTime.Id,
                        PlayerNo = racePlayer.No,
                        Status = racePlayerTime.Status,
                    });
                }
                else
                {
                    // Duplicate
                    racePlayerTime.Status = PlayerLogTimeStatus.Duplicate;
                    _racePlayerTimeRepository.Update(racePlayerTime);

                    _racePlayerTimeUpdated.OnNext(new RacePlayerTimeViewModel
                    {
                        Id = racePlayerTime.Id,
                        Status = racePlayerTime.Status,
                    });
                }
            }
            else
            {
                // Non-existent player
                racePlayerTime.Status = PlayerLogTimeStatus.PlayerNonExistent;
                _racePlayerTimeRepository.Update(racePlayerTime);

                _racePlayerTimeUpdated.OnNext(new RacePlayerTimeViewModel
                {
                    Id = racePlayerTime.Id,
                    Status = racePlayerTime.Status,
                });
            }
        }

        public void DeletePlayerLogTime(string raceId, string racePlayerTimeId)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace();
            Guard.Argument(racePlayerTimeId, nameof(racePlayerTimeId))
                .NotNull().NotEmpty().NotWhiteSpace();

            _racePlayerTimeRepository.Delete(raceId, racePlayerTimeId);

            _racePlayerTimeDeleted.OnNext(racePlayerTimeId);
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

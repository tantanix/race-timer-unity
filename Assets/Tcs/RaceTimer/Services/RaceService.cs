using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Tcs.RaceTimer.ViewModels;
using Tcs.RaceTimer.Exceptions;

namespace Tcs.RaceTimer.Services
{
    public class RaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly RaceCategoryRepository _raceCategoryRepository;
        private readonly RacePlayerRepository _racePlayerRepository;

        private readonly BehaviorSubject<Race> _currentRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<RaceCategory> _currentRaceCategory = new BehaviorSubject<RaceCategory>(null);

        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<Player> _newPlayer = new BehaviorSubject<Player>(null);
        private readonly BehaviorSubject<Team> _newTeam = new BehaviorSubject<Team>(null);
        private readonly BehaviorSubject<Category> _newCategory = new BehaviorSubject<Category>(null);

        private readonly BehaviorSubject<RaceCategoryViewModel> _newRaceCategory = new BehaviorSubject<RaceCategoryViewModel>(null);
        private readonly BehaviorSubject<RacePlayerViewModel> _newRacePlayer = new BehaviorSubject<RacePlayerViewModel>(null);

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

        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository,
            CategoryRepository categoryRepository,
            RaceCategoryRepository raceCategoryRepository,
            RacePlayerRepository racePlayerRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _categoryRepository = categoryRepository;
            _raceCategoryRepository = raceCategoryRepository;
            _racePlayerRepository = racePlayerRepository;
        }

        public void LoadRace(string raceId)
        {
            CurrentRace = _raceRepository.Get(raceId);
            _currentRace.OnNext(CurrentRace);

            // Load first category
            var raceCategories = _raceCategoryRepository.GetAll(raceId);
            if (raceCategories != null && raceCategories.Any())
            {
                var firstCategory = raceCategories.First();
                LoadRaceCategory(firstCategory.Id);
            }
        }

        public void LoadRaceCategory(string raceCategoryId)
        {
            CurrentRaceCategory = _raceCategoryRepository.Get(raceCategoryId);
            _currentRaceCategory.OnNext(CurrentRaceCategory);
        }

        public Race CreateRace(string name, long eventDate, int stages, string location)
        {
            var id = Guid.NewGuid().ToString();
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

        public Race CreateRace(string id, string name, long eventDate, int stages, string location)
        {
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

        public IEnumerable<Race> GetAllRaces()
        {
            return _raceRepository.GetAll();
        }

        public Team CreateTeam(string name)
        {
            var newId = Guid.NewGuid().ToString();

            return CreateTeam(newId, name);
        }

        public Team CreateTeam(string id, string name)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Team id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Team name cannot be null or empty or whitespace");

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
            var newId = Guid.NewGuid().ToString();
            return CreatePlayer(newId, name, age, email);
        }

        public Player CreatePlayer(string id, string name, int age, string email)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Player id cannot be null or empty or whitespace");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name cannot be null or empty or whitespace.");

            if (age <= 0)
                throw new ArgumentException("age cannot be zero or less.");

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email cannot be null or empty or whitespace.");

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

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name cannot be null or empty or whitespace.");

            if (age <= 0)
                throw new ArgumentException("age cannot be zero or less.");

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email cannot be null or empty or whitespace.");

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
                    Player = _playerRepository.Get(rp.PlayerId)
                });
        }

        public IEnumerable<RacePlayerViewModel> GetAllRaceCategoryPlayers()
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

            if (CurrentRaceCategory == null)
                throw new RaceCategoryNotLoadedException();

            var racePlayers = _racePlayerRepository
                .GetAll(CurrentRace.Id)
                .Where(x => x.CategoryId == CurrentRaceCategory.CategoryId);

            return racePlayers.Select(rp =>
                new RacePlayerViewModel
                {
                    Id = rp.Id,
                    Race = CurrentRace,
                    Category = _categoryRepository.Get(rp.CategoryId),
                    Team = _teamRepository.Get(rp.TeamId),
                    Player = _playerRepository.Get(rp.PlayerId)
                });
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _playerRepository.GetAll();
        }

        public Category CreateCategory(string name)
        {
            var categoryId = Guid.NewGuid().ToString();
            return CreateCategory(categoryId, name);
        }

        public Category CreateCategory(string id, string name)
        {
            var category = _categoryRepository.Create(new Category
            {
                Id = id,
                Name = name
            });
            _newCategory.OnNext(category);
            return category;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
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

            var raceCategoryPlayers = _racePlayerRepository.GetAll(CurrentRace.Id).Where(x => x.CategoryId == categoryId);
            var result = raceCategoryPlayers
                .Select(x => new RacePlayerViewModel
                {
                    Category = _categoryRepository.Get(x.CategoryId),
                    Player = _playerRepository.Get(x.PlayerId),
                    Team = _teamRepository.Get(x.TeamId),
                    Race = _raceRepository.Get(x.RaceId)
                });

            return result;
        }

        public RaceCategory AddRaceCategory(string categoryName)
        {
            if (CurrentRace == null)
                throw new RaceNotLoadedException();

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

            return raceCategory;
        }
    }
}

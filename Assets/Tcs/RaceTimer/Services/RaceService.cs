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

        private readonly BehaviorSubject<RaceViewModel> _currentRace = new BehaviorSubject<RaceViewModel>(null);
        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<Player> _newPlayer = new BehaviorSubject<Player>(null);
        private readonly BehaviorSubject<Team> _newTeam = new BehaviorSubject<Team>(null);
        private readonly BehaviorSubject<Category> _newCategory = new BehaviorSubject<Category>(null);

        private readonly BehaviorSubject<RaceCategoryViewModel> _newRaceCategory = new BehaviorSubject<RaceCategoryViewModel>(null);
        private readonly BehaviorSubject<RacePlayerViewModel> _newRacePlayer = new BehaviorSubject<RacePlayerViewModel>(null);

        public RaceViewModel CurrentRace { get; private set; }
        public RaceCategory CurrentCategory { get; private set; }

        public IObservable<RaceViewModel> OnRaceLoaded() => _currentRace.AsObservable();
        public IObservable<Race> OnNewRace() => _newRace.AsObservable();
        public IObservable<Player> OnNewPlayer() => _newPlayer.AsObservable();
        public IObservable<Team> OnNewTeam() => _newTeam.AsObservable();
        public IObservable<Category> OnNewCategory() => _newCategory.AsObservable();
        public IObservable<RacePlayerViewModel> OnNewRacePlayer() => _newRacePlayer.AsObservable();
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
            var race = _raceRepository.Get(raceId);
            var players = GetAllPlayers();
            var racePlayers = GetAllRacePlayers(race.Id);
            var raceCategories = GetAllRaceCategories(race.Id);
            
            CurrentRace = new RaceViewModel
            {
                Race = race,
                Players = players.ToList(),
                RacePlayers = racePlayers.ToList(),
                RaceCategories = raceCategories.ToList()
            };

            _currentRace.OnNext(CurrentRace);
        }

        public void LoadRaceCategory(string raceCategoryId)
        {
            _raceCategoryRepository.Get(raceCategoryId);
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

        public RacePlayer CreateRacePlayer(string raceId, string name, int age, string email, string teamName)
        {
            var racePlayerId = Guid.NewGuid().ToString();
            return CreateRacePlayer(racePlayerId, raceId, name, age, email, teamName);
        }

        public RacePlayer CreateRacePlayer(string id, string raceId, string name, int age, string email, string teamName)
        {
            if (string.IsNullOrEmpty(raceId) || string.IsNullOrWhiteSpace(raceId))
                throw new ArgumentException("raceId cannot be null or empty or whitespace.");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name cannot be null or empty or whitespace.");

            if (age <= 0)
                throw new ArgumentException("age cannot be zero or less.");

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email cannot be null or empty or whitespace.");

            if (string.IsNullOrEmpty(teamName) || string.IsNullOrWhiteSpace(teamName))
                throw new ArgumentException("teamName cannot be null or empty or whitespace.");

            var player = _playerRepository.FindByName(name);
            if (player == null)
            {
                player = CreatePlayer(name, age, email);
            }

            var team = _teamRepository.FindByName(teamName);
            if (team == null)
            {
                var teamId = Guid.NewGuid().ToString();
                team = CreateTeam(teamId, teamName);
            }

            var racePlayer = _racePlayerRepository.Find(raceId, team.Id, player.Id);
            if (racePlayer == null)
            {
                racePlayer = _racePlayerRepository.Create(
                    raceId,
                    new RacePlayer
                    {
                        Id = id,
                        TeamId = team.Id,
                        PlayerId = player.Id
                    });
            }

            _newRacePlayer.OnNext(
                new RacePlayerViewModel
                {
                    Id = racePlayer.Id,
                    Race = CurrentRace.Race,
                    Team = team,
                    Player = player
                });

            return racePlayer;
        }

        public IEnumerable<RacePlayerViewModel> GetAllRacePlayers(string raceId)
        {
            var racePlayers = _racePlayerRepository.GetAll(raceId);
            return racePlayers.Select(rp => 
                new RacePlayerViewModel
                {
                    Id = rp.Id,
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

        public IEnumerable<RaceCategoryViewModel> GetAllRaceCategories(string raceId)
        {
            var raceCategories = _raceCategoryRepository.GetAll(raceId);
            var result = raceCategories
                .Select(x => new RaceCategoryViewModel
                {
                    Race = _raceRepository.Get(x.RaceId),
                    Category = _categoryRepository.Get(x.CategoryId)
                });
            return result;
        }

        public IEnumerable<RacePlayerViewModel> GetAllRaceCategoryPlayers(string raceId, string categoryId)
        {
            var raceCategoryPlayers = _racePlayerRepository.GetAll(raceId).Where(x => x.CategoryId == categoryId);
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

        public RaceCategory AddRaceCategory(string raceId, string categoryName)
        {
            if (string.IsNullOrEmpty(raceId) || string.IsNullOrEmpty(categoryName))
                return null;

            var race = _raceRepository.Get(raceId);
            var category = _categoryRepository.FindByName(categoryName);
            if (category == null)
            {
                category = CreateCategory(categoryName);
            }

            // Don't add it if it's already been added
            var exists = _raceCategoryRepository.FindByCategory(raceId, category.Id);
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
                    Race = race,
                    Category = category
                });

            return raceCategory;
        }
    }
}

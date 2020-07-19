using Tcs.RaceTimer.Interfaces;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Tcs.RaceTimer.ViewModels;
using UnityEngine;

namespace Tcs.RaceTimer.Services
{
    public class RaceService : IRaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly RaceCategoryRepository _raceCategoryRepository;
        private readonly RacePlayerRepository _racePlayerRepository;

        private readonly BehaviorSubject<Race> _currentRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<Player> _newPlayer = new BehaviorSubject<Player>(null);
        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<RacePlayerInfo> _newRacePlayer = new BehaviorSubject<RacePlayerInfo>(null);

        public Race CurrentRace { get; private set; }

        public IObservable<Race> OnRaceLoaded { get; private set; }
        public IObservable<Race> OnNewRace { get; private set; }
        public IObservable<Player> OnNewPlayer { get; private set; }
        public IObservable<RacePlayerInfo> OnNewRacePlayer { get; private set; }

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

            Initialize();
        }

        private void Initialize()
        {
            OnRaceLoaded = _currentRace.AsObservable();
            OnNewRace = _newRace.AsObservable();
            OnNewPlayer = _newPlayer.AsObservable();
            OnNewRacePlayer = _newRacePlayer.AsObservable();
        }

        public PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time)
        {
            throw new NotImplementedException();
        }

        public RacePlayer AddTeamPlayer(string raceId, string teamId, string playerId)
        {
            throw new NotImplementedException();
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
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Team name cannot be null or empty or whitespace");

            var newId = Guid.NewGuid().ToString();
            var team = _teamRepository.Create(
                new Team
                {
                    Id = newId,
                    Name = name
                });

            return team;
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

            return team;
        }

        public void LoadRace(string raceId)
        {
            CurrentRace = _raceRepository.Get(raceId);
            _currentRace.OnNext(CurrentRace);
        }

        public Player CreatePlayer(string name, int age, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name cannot be null or empty or whitespace.");

            if (age <= 0)
                throw new ArgumentException("age cannot be zero or less.");

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email cannot be null or empty or whitespace.");

            var newId = Guid.NewGuid().ToString();
            var player = _playerRepository.Create(
                new Player {
                    Id = newId,
                    Name = name,
                    Age = age,
                    Email = email
                });
            return player;
        }

        public Player CreatePlayer(string id, string name, int age, string email)
        {
            throw new NotImplementedException();
        }

        public RacePlayerInfo CreateRacePlayer(string raceId, string name, int age, string email, string teamName)
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
                _newPlayer.OnNext(player);
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
                var racePlayerId = Guid.NewGuid().ToString();
                racePlayer = _racePlayerRepository.Create(
                    raceId,
                    new RacePlayer
                    {
                        Id = racePlayerId,
                        TeamId = team.Id,
                        PlayerId = player.Id
                    });
            }

            var racePlayerInfo = new RacePlayerInfo
            {
                Id = racePlayer.Id,
                Race = CurrentRace,
                Team = team,
                Player = player
            };

            _newRacePlayer.OnNext(racePlayerInfo);

            return racePlayerInfo;
        }

        public RacePlayerInfo CreateRacePlayer(string id, string raceId, string name, int age, string email, string teamName, int no)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RacePlayerInfo> GetAllRacePlayers(string raceId)
        {
            var racePlayers = _racePlayerRepository.GetAll(raceId);
            return racePlayers.Select(rp => new RacePlayerInfo
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
            throw new NotImplementedException();
        }

        public Category CreateCategory(string id, string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryRepository.GetAll();
        }

        public IEnumerable<Category> GetAllRaceCategories(string raceId)
        {
            var raceCategories = _raceCategoryRepository.GetAll(raceId);
            return raceCategories.Select(x => _categoryRepository.Get(x.Id));
        }

        public IEnumerable<CategoryPlayer> GetAllRaceCategoryPlayers(string raceId, string categoryId)
        {
            throw new NotImplementedException();
        }

    }
}

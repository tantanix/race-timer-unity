﻿using Tcs.RaceTimer.Interfaces;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Tcs.RaceTimer.Services
{
    public class RaceService : IRaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly RacePlayerRepository _racePlayerRepository;

        private readonly BehaviorSubject<Player> _newPlayer = new BehaviorSubject<Player>(null);
        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        private readonly BehaviorSubject<RacePlayerInfo> _newRacePlayer = new BehaviorSubject<RacePlayerInfo>(null);

        public Race CurrentRace { get; private set; }

        public IObservable<Race> OnNewRace { get; private set; }
        public IObservable<Player> OnNewPlayer { get; private set; }
        public IObservable<RacePlayerInfo> OnNewRacePlayer { get; private set; }

        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository,
            CategoryRepository categoryRepository,
            RacePlayerRepository racePlayerRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _categoryRepository = categoryRepository;
            _racePlayerRepository = racePlayerRepository;

            Initialize();
        }

        private void Initialize()
        {
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
            var newRace = _raceRepository.Create(id, name, eventDate, stages, location);
            _newRace.OnNext(newRace);
            return newRace;
        }

        public Race CreateRace(string id, string name, long eventDate, int stages, string location)
        {
            var newRace = _raceRepository.Create(id, name, eventDate, stages, location);
            _newRace.OnNext(newRace);
            return newRace;
        }

        public IEnumerable<Race> GetAllRaces()
        {
            return _raceRepository.GetAll();
        }

        public Team CreateTeam(string id, string name)
        {
            return _teamRepository.Create(id, name);
        }

        public void LoadRace(string raceId)
        {
            CurrentRace = _raceRepository.Get(raceId);

        }

        public RacePlayerInfo CreatePlayer(string raceId, string name, int age, string email, string teamName)
        {
            var player = _playerRepository.FindByName(name);
            if (player == null)
            {
                var playerId = Guid.NewGuid().ToString();
                player = _playerRepository.Create(playerId, name, age, email);

                _newPlayer.OnNext(player);
            }

            var team = _teamRepository.FindByName(teamName);
            if (team == null)
            {
                var teamId = Guid.NewGuid().ToString();
                team = _teamRepository.Create(teamId, teamName);
            }

            var racePlayer = _racePlayerRepository.Find(raceId, team.Id, player.Id);
            if (racePlayer == null)
            {
                var racePlayerId = Guid.NewGuid().ToString();
                racePlayer = _racePlayerRepository.Create(racePlayerId, raceId, team.Id, player.Id);
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

        public Player CreatePlayer(string id, string name, int age, string email, string teamName, int no)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RacePlayerInfo> GetAllRacePlayers(string raceId)
        {
            var racePlayers = _racePlayerRepository.GetAll(raceId);
            return racePlayers.Select(rp => new RacePlayerInfo
                {
                    Id = rp.Id,
                    Race = _raceRepository.Get(rp.RaceId),
                    Team = _teamRepository.Get(rp.TeamId),
                    Player = _playerRepository.Get(rp.PlayerId)
                });
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _playerRepository.GetAll();
        }
    }
}

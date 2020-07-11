using Tcs.RaceTimer.Interfaces;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using Tcs.Observables;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Services
{
    public class RaceService : IRaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;

        private readonly BehaviorSubject<Race> _newRace = new BehaviorSubject<Race>(null);
        
        public Race CurrentRace { get; private set; }

        public IObservable<Race> OnNewRace { get; private set; }
        
        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;

            Initialize();
        }

        private void Initialize()
        {
            OnNewRace = _newRace.AsObservable();
        }

        public PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time)
        {
            throw new NotImplementedException();
        }

        public TeamPlayer AddTeamPlayer(string raceId, string teamId, string playerId)
        {
            throw new NotImplementedException();
        }

        public Player CreatePlayer(string id, string name, string no)
        {
            throw new NotImplementedException();
        }

        public Race CreateRace(string name, long eventDate, int stages)
        {
            var newRace = _raceRepository.CreateRace(name, eventDate, stages);
            _newRace.Next(newRace);
            return newRace;
        }

        public Race CreateRace(string id, string name, long eventDate, int stages)
        {
            var newRace = _raceRepository.CreateRace(id, name, eventDate, stages);
            _newRace.Next(newRace);
            return newRace;
        }

        public IEnumerable<Race> GetAllRaces()
        {
            return _raceRepository.GetAll();
        }

        public Team CreateTeam(string id, string name)
        {
            return _teamRepository.CreateTeam(id, name);
        }

        public void LoadRace(string raceId)
        {
            CurrentRace = _raceRepository.GetRace(raceId);

        }
    }
}

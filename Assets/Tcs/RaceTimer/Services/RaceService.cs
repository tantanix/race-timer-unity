using Tcs.RaceTimer.Interfaces;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Services
{
    public class RaceService : IRaceService, IObservable<Race>
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;

        private readonly List<IObserver<Race>> _observers;

        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;

            _observers = new List<IObserver<Race>>();
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
            foreach (var observer in _observers)
            {
                observer.OnNext(newRace);
            }

            return newRace;
        }

        public Race CreateRace(string id, string name, long eventDate, int stages)
        {
            var newRace = _raceRepository.CreateRace(id, name, eventDate, stages);
            foreach (var observer in _observers)
            {
                observer.OnNext(newRace);
            }

            return newRace;
        }

        public Team CreateTeam(string id, string name)
        {
            return _teamRepository.CreateTeam(id, name);
        }

        public IDisposable Subscribe(IObserver<Race> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                
                foreach (var item in _raceRepository.GetAll())
                    observer.OnNext(item);
            }

            return new Subscription<Race>(_observers, observer);
        }
    }
}

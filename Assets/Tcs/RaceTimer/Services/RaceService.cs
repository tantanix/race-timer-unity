using Assets.Tcs.RaceTimer.Interfaces;
using Assets.Tcs.RaceTimer.Models;
using Assets.Tcs.RaceTimer.Repository;
using System;
using System.Collections.Generic;

namespace Assets.Tcs.RaceTimer.Services
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

        public PlayerTime AddPlayerTime(Guid raceId, Guid playerId, TimeType type, LogTime time)
        {
            throw new NotImplementedException();
        }

        public TeamPlayer AddTeamPlayer(Guid raceId, Guid teamId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Player CreatePlayer(Guid id, string name, string no)
        {
            throw new NotImplementedException();
        }

        public Race CreateRace(Guid id, string name, DateTime eventDate, int stages)
        {
            throw new NotImplementedException();
        }

        public Race CreateRace(Race race)
        {
            var newRace = _raceRepository.CreateRace(race.Name, race.EventDate, race.Stages);
            foreach (var observer in _observers)
            {
                observer.OnNext(newRace);
            }

            return newRace;
        }

        public Team CreateTeam(Guid id, string name)
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

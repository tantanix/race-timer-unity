using Assets.Tcs.RaceTimer.Interfaces;
using Assets.Tcs.RaceTimer.Models;
using Assets.Tcs.RaceTimer.Repository;
using System;

namespace Assets.Tcs.RaceTimer.Services
{
    public class RaceService : IRaceService
    {
        private readonly RaceRepository _raceRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;

        public RaceService(
            RaceRepository raceRepository,
            PlayerRepository playerRepository,
            TeamRepository teamRepository)
        {
            _raceRepository = raceRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
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

        public Race CreateRace(string id, string name, DateTime eventDate, int stages)
        {
            throw new NotImplementedException();
        }

        public Team CreateTeam(string id, string name)
        {
            return this._teamRepository.CreateTeam(id, name);
        }

        public void SaveRace(string raceId)
        {
            var race = this._raceRepository.GetRace(raceId);
            
        }
    }
}

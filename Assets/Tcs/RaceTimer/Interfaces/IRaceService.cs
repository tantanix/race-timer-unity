using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Interfaces
{
    public interface IRaceService
    {
        Race CurrentRace { get; }
        IObservable<Race> OnNewRace { get; }

        Race CreateRace(string name, long eventDate, int stages, string location);
        Race CreateRace(string id, string name, long eventDate, int stages, string location);
        IEnumerable<Race> GetAllRaces();
        Player CreatePlayer(string id, string name, string no);
        Team CreateTeam(string id, string name);
        void LoadRace(string raceId);
        TeamPlayer AddTeamPlayer(string raceId, string teamId, string playerId);
        PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time);
    }
}

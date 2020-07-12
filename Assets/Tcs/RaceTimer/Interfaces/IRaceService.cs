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
        RacePlayer CreatePlayer(string raceId, string name, int age, string email, string teamName);
        Player CreatePlayer(string id, string name, int age, string email, string teamName, int no);
        Team CreateTeam(string id, string name);
        void LoadRace(string raceId);
        RacePlayer AddTeamPlayer(string raceId, string teamId, string playerId);
        PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time);
    }
}

using Assets.Tcs.RaceTimer.Models;
using System;

namespace Assets.Tcs.RaceTimer.Interfaces
{
    public interface IRaceService
    {
        Race CreateRace(string id, string name, DateTime eventDate, int stages);
        Player CreatePlayer(string id, string name, string no);
        Team CreateTeam(string id, string name);
        TeamPlayer AddTeamPlayer(string raceId, string teamId, string playerId);
        PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, Time time);
        void SaveRace(string raceId);
    }
}

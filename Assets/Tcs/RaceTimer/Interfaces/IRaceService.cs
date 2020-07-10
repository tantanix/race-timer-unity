using Tcs.RaceTimer.Models;
using System;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Interfaces
{
    public interface IRaceService
    {
        Race CreateRace(string name, long eventDate, int stages);
        Race CreateRace(string id, string name, long eventDate, int stages);
        Player CreatePlayer(string id, string name, string no);
        Team CreateTeam(string id, string name);
        TeamPlayer AddTeamPlayer(string raceId, string teamId, string playerId);
        PlayerTime AddPlayerTime(string raceId, string playerId, TimeType type, LogTime time);
    }
}

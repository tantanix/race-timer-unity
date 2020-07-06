using Assets.Tcs.RaceTimer.Models;
using System;

namespace Assets.Tcs.RaceTimer.Interfaces
{
    public interface IRaceService
    {
        Race CreateRace(Guid id, string name, DateTime eventDate, int stages);
        Race CreateRace(Race race);
        Player CreatePlayer(Guid id, string name, string no);
        Team CreateTeam(Guid id, string name);
        TeamPlayer AddTeamPlayer(Guid raceId, Guid teamId, Guid playerId);
        PlayerTime AddPlayerTime(Guid raceId, Guid playerId, TimeType type, LogTime time);
    }
}

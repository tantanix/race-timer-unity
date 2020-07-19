using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RacePlayerRepository : EntityReferenceRepository<RacePlayer, RacePlayerList>
    {
        public RacePlayerRepository() : base("RacePlayerList-") { }

        public RacePlayer Find(string raceId, string teamId, string playerId)
        {
            var racePlayers = GetAll(raceId);
            foreach (var racePlayer in racePlayers)
            {
                if (racePlayer.TeamId == teamId && racePlayer.PlayerId == playerId)
                    return racePlayer;
            }

            return null;
        }
    }
}

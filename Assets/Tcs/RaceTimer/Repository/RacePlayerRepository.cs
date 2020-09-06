using System.Linq;
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
            return racePlayers.SingleOrDefault(x => x.RaceId == raceId && x.TeamId == teamId && x.PlayerId == playerId);
        }

        public RacePlayer Find(string raceId, string teamId, string categoryId, string playerId)
        {
            var racePlayers = GetAll(raceId);
            return racePlayers.SingleOrDefault(x => x.RaceId == raceId && x.TeamId == teamId && x.CategoryId == categoryId && x.PlayerId == playerId);
        }
    }
}

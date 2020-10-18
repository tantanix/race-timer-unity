using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class PlayerRepository : EntityRepository<Player, PlayerList>
    {
        public PlayerRepository() : base("PlayerListIds", "P-") { }

        public Player FindByName(string name)
        {
            var players = GetAll();
            foreach (var player in players)
            {
                if (player.Name == name)
                    return player;
            }

            return null;
        }
    }
}

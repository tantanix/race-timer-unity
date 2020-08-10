using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class PlayerRepository : EntityRepository<Player, PlayerList>
    {
        private const string IdPrefix = "Player-";

        public PlayerRepository() : base("PlayerListIds") { }

        public string GenerateId()
        {
            return $"{IdPrefix}{System.Guid.NewGuid()}";
        }

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

        public override Player Get(string id)
        {
            if (string.IsNullOrEmpty(id) || !id.Contains(IdPrefix))
                throw new EntityNotFoundException<Player>();

            return base.Get(id);
        }
    }
}

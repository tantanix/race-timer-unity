using Tcs.RaceTimer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class PlayerRepository
    {
        private readonly List<Player> players = new List<Player>();

        public Player CreatePlayer(Guid id, string name, string no)
        {
            var player = new Player
            {
                Id = id,
                Name = name,
                No = no
            };

            this.players.Add(player);

            return player;
        }

        public Player Get(Guid id)
        {
            var player = this.players.FirstOrDefault(p => Equals(p.Id, id));
            if (player == null)
                throw new PlayerNotFoundException();

            return player;
        }

        public Player FindByName(string name)
        {
            var player = this.players.FirstOrDefault(p => p.Name == name);
            if (player == null)
                throw new PlayerNotFoundException();

            return player;
        }

        public Player FindByNo(string no)
        {
            var player = this.players.FirstOrDefault(p => p.No == no);
            if (player == null)
                throw new PlayerNotFoundException();

            return player;
        }

        public List<Player> Search(string keyword)
        {
            return null;
        }
    }
}

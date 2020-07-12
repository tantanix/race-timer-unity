using System;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RaceData
    {
        public Race Race;
        public List<Player> Players;
        public List<Team> Teams;
        public List<RacePlayer> TeamPlayers;
        public List<PlayerTime> PlayerTimes;
    }
}

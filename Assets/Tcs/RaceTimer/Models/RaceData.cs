using System.Collections.Generic;

namespace Assets.Tcs.RaceTimer.Models
{
    public class RaceData
    {
        public Race Race { get; set; }
        public List<Player> Players { get; set; }
        public List<Team> Teams { get; set; }
        public List<TeamPlayer> TeamPlayers { get; set; }
        public List<PlayerTime> PlayerTimes { get; set; }
    }
}

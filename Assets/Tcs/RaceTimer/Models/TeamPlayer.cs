using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class TeamPlayer
    {
        public Race Race;
        public Team Team;
        public Player Player;
    }
}

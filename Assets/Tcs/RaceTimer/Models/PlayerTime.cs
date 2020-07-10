using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class PlayerTime
    {
        public Race Race;
        public Player Player;
        public TimeType Type;
        public int Stage;
        public LogTime Time;
    }
}

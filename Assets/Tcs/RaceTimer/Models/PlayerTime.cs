using System.Collections.Generic;

namespace Assets.Tcs.RaceTimer.Models
{
    public class PlayerTime
    {
        public Race Race { get; set; }
        public Player Player { get; set; }
        public TimeType Type { get; set; }
        public int Stage { get; set; }
        public LogTime Time { get; set; }
    }
}

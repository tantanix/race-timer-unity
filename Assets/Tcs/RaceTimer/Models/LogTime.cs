using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public struct LogTime
    {
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Milliseconds;
    }
}

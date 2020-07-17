using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RacePlayer
    {
        public string Id;
        public string RaceId;
        public string TeamId;
        public string PlayerId;
        public int No;
        public long StartTime;
    }
}

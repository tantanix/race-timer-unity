using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class Race
    {
        public string Id;
        public string Name;
        public long EventDate;
        public int Stages;
        public string Location;
    }
}
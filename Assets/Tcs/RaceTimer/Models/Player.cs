using System;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class Player
    {
        public Guid Id;
        public string Name;
        public string Team;
        public string No;
    }
}

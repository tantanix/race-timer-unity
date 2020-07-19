using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RacePlayer : Entity
    {
        public string TeamId;
        public string PlayerId;
        public int No;
        public long StartTime;
    }
}

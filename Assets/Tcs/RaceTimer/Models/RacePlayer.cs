using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RacePlayer : Entity
    {
        public string RaceId;
        public string PlayerId;
        public string TeamId;
        public string CategoryId;
        public int No;
        public long StartTime;
    }
}

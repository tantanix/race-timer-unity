using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RacePlayerTime : Entity
    {
        public string RaceId;
        public string PlayerId;
        public string CategoryId;
        public TimeType Type;
        public int Stage;
        public LogTime Time;
    }
}

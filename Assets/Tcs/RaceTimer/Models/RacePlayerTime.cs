using System;
using Tcs.Core.Entity;
using Tcs.RaceTimer.Enums;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RacePlayerTime : Entity
    {
        public string RaceId;
        public string RacePlayerId;
        public string CategoryId;
        public TimeType Type;
        public int Stage;
        public LogTime Time;
        public PlayerLogTimeStatus Status;
    }
}

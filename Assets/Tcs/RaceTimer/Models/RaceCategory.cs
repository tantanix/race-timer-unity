using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RaceCategory : Entity
    {
        public string RaceId;
        public string CategoryId;
    }
}

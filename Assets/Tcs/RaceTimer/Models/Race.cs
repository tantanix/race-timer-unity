using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class Race : Entity
    {
        public string Name;
        public long EventDate;
        public int Stages;
        public string Location;
    }
}
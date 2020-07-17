using System;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class RaceData
    {
        public Race Race;
        public List<Category> Categories;
        public List<RacePlayer> RacePlayers;
        public List<PlayerTime> PlayerTimes;
    }
}

using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.ViewModels
{
    [Serializable]
    public class RaceData
    {
        public Race Race;
        public List<Category> Categories;
        public List<RacePlayer> Players;
        public List<PlayerTime> PlayerTimes;
    }
}

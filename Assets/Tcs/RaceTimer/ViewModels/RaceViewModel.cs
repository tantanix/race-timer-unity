using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.ViewModels
{
    [Serializable]
    public class RaceViewModel
    {
        public Race Race;
        public List<Player> Players;
        public List<RaceCategoryViewModel> RaceCategories;
        public List<RacePlayerViewModel> RacePlayers;
        public List<PlayerTime> PlayerTimes;
    }
}

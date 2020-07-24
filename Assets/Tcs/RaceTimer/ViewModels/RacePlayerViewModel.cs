using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.ViewModels
{
    [System.Serializable]
    public class RacePlayerViewModel
    {
        public string Id;
        public Race Race;
        public Category Category;
        public Team Team;
        public Player Player;
        public int No;
        public long StartTime;
    }
}

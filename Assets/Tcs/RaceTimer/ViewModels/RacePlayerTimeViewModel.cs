using Tcs.RaceTimer.Enums;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.ViewModels
{
    [System.Serializable]
    public class RacePlayerTimeViewModel
    {
        public string Id;
        public LogTime? Time;
        public int? PlayerNo;
        public PlayerLogTimeStatus? Status;
    }
}

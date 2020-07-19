using System;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Models
{
    [Serializable]
    public class Player : Entity
    {
        public string Name;
        public int Age;
        public string Email;
    }
}

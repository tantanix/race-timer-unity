using Tcs.RaceTimer.Models;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Repository
{
    public class RaceRepository : EntityRepository<Race, RaceList>
    {
        public RaceRepository() : base("RaceListIds") { }
    }
}

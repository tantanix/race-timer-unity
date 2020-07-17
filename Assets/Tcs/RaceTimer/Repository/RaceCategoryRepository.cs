using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RaceCategoryRepository : EntityReferenceRepository<RaceCategory, RaceCategoryList>
    {
        public RaceCategoryRepository() : base("RaceCategory-") { }
    }
}

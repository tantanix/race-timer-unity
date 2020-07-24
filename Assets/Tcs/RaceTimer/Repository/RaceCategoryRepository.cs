using System;
using System.Linq;
using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RaceCategoryRepository : EntityReferenceRepository<RaceCategory, RaceCategoryList>
    {
        public RaceCategoryRepository() : base("RaceCategory-") { }

        public RaceCategory FindByCategory(string raceId, string categoryId)
        {
            var all = GetAll(raceId);
            
            return all.FirstOrDefault(x => x.CategoryId == categoryId);
        }
    }
}

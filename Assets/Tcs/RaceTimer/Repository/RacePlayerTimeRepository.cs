using System;
using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RacePlayerTimeRepository : EntityReferenceRepository<RacePlayerTime, RacePlayerTimeList>
    {
        public RacePlayerTimeRepository() : base("RacePlayerTimeList-") { }

        public RacePlayerTime Find(string raceId, string categoryId, string playerId, int stage, TimeType type)
        {
            var racePlayerTimes = GetAll(raceId);
            foreach (var racePlayerTime in racePlayerTimes)
            {
                if (racePlayerTime.RaceId == raceId && 
                    racePlayerTime.CategoryId == categoryId && 
                    racePlayerTime.PlayerId == playerId &&
                    racePlayerTime.Stage == stage &&
                    racePlayerTime.Type == type)

                    return racePlayerTime;
            }

            return null;
        }

        public RacePlayerTime CreateOrUpdate(RacePlayerTime model)
        {
            var rpt = Find(model.RaceId, model.CategoryId, model.PlayerId, model.Stage, model.Type);
            if (rpt == null)
            {
                return Create(model.RaceId, model);
            }

            rpt.Time = model.Time;
            return Update(model.RaceId, model);
        }

    }
}

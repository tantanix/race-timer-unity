using System.Collections.Generic;
using System.Linq;
using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class RacePlayerTimeRepository : EntityReferenceRepository<RacePlayerTime, RacePlayerTimeList>
    {
        public RacePlayerTimeRepository() : base("RacePlayerTimeList-") { }

        public RacePlayerTime Find(string raceId, string categoryId, string playerId, int stage, TimeType type)
        {
            var racePlayerTime = GetAll(raceId)
                .FirstOrDefault(x =>
                    x.RaceId == raceId &&
                    x.CategoryId == categoryId &&
                    x.PlayerId == playerId &&
                    x.Stage == stage &&
                    x.Type == type);

            return racePlayerTime;
        }

        public IEnumerable<RacePlayerTime> GetAllByRaceCategoryPlayer(string raceId, string categoryId, string playerId)
        {
            var racePlayerTimes = GetAll(raceId)
                .Where(racePlayerTime =>
                    racePlayerTime.RaceId == raceId &&
                    racePlayerTime.CategoryId == categoryId &&
                    racePlayerTime.PlayerId == playerId);

            return racePlayerTimes;
        }

        public RacePlayerTime CreateOrUpdate(RacePlayerTime model)
        {
            var rpt = Find(model.RaceId, model.CategoryId, model.PlayerId, model.Stage, model.Type);
            if (rpt == null)
            {
                return Create(model.RaceId, model);
            }

            rpt.Time = model.Time;
            return Update(model.RaceId, rpt);
        }

    }
}

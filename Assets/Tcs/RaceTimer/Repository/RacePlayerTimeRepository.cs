using Dawn;
using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.Core.Entity;
using Tcs.RaceTimer.Enums;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    internal class LogTimeComparer : IComparer<LogTime>
    {
        public int Compare(LogTime x, LogTime y)
        {
            if (x.Hours > y.Hours) return 1;
            else if (x.Hours < y.Hours) return -1;
            else
            {
                if (x.Minutes > y.Minutes) return 1;
                else if (x.Minutes < y.Minutes) return -1;
                else
                {
                    if (x.Seconds > y.Seconds) return 1;
                    else if (x.Seconds < y.Seconds) return -1;
                    else
                    {
                        if (x.Milliseconds > y.Milliseconds) return 1;
                        else if (x.Milliseconds < y.Milliseconds) return -1;
                        else return 0;
                    }
                }
            }
        }
    }

    public class RacePlayerTimeRepository : EntityReferenceRepository<RacePlayerTime, RacePlayerTimeList>
    {
        private readonly LogTimeComparer DefaultLogTimeComparer = new LogTimeComparer();

        public RacePlayerTimeRepository() : base("RacePlayerTimeList-", "RPT-") { }

        public RacePlayerTime Find(string raceId, string categoryId, string racePlayerId, int stage, TimeType type)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(racePlayerId, nameof(racePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("RP-");
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("C-");

            var racePlayerTime = GetAll(raceId)
                .FirstOrDefault(x =>
                    x.RaceId == raceId &&
                    x.CategoryId == categoryId &&
                    x.RacePlayerId == racePlayerId &&
                    x.Stage == stage &&
                    x.Type == type);

            return racePlayerTime;
        }

        public IEnumerable<RacePlayerTime> GetAllUnassignedByRaceAndStage(string raceId, int stage, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(stage, nameof(stage))
                .GreaterThan(0).LessThan(6);

            var unassignedRacePlayerTimes = GetAll(raceId)
                .Where(rpt =>
                    rpt.Stage == stage &&
                    rpt.Type == timeType &&
                    rpt.Status == PlayerLogTimeStatus.NotSet)
                .OrderBy(rpt => rpt.Time, DefaultLogTimeComparer);

            return unassignedRacePlayerTimes;
        }

        public IEnumerable<RacePlayerTime> GetAllByRaceCategoryPlayer(string raceId, string categoryId, string racePlayerId)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(racePlayerId, nameof(racePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("RP-");
            Guard.Argument(categoryId, nameof(categoryId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("C-");

            var racePlayerTimes = GetAll(raceId)
                .Where(racePlayerTime =>
                    racePlayerTime.RaceId == raceId &&
                    racePlayerTime.CategoryId == categoryId &&
                    racePlayerTime.RacePlayerId == racePlayerId);

            return racePlayerTimes;
        }

        public IEnumerable<RacePlayerTime> GetAllByRaceAndStage(string raceId, int stage, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(stage, nameof(stage))
                .GreaterThan(0).LessThan(6);

            var racePlayerTimes = GetAll(raceId)
                .Where(racePlayerTime =>
                    racePlayerTime.RaceId == raceId &&
                    racePlayerTime.Stage == stage &&
                    racePlayerTime.Type == timeType);

            return racePlayerTimes;
        }

        public RacePlayerTime GetByRaceAndStageAndPlayerNo(string raceId, int stage, string racePlayerId, TimeType timeType)
        {
            Guard.Argument(raceId, nameof(raceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(stage, nameof(stage))
                .GreaterThan(0).LessThan(6);
            Guard.Argument(racePlayerId, nameof(racePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("RP-");
            
            var racePlayerTime = GetAll(raceId)
                .SingleOrDefault(rpt =>
                    rpt.RaceId == raceId &&
                    rpt.Stage == stage &&
                    rpt.RacePlayerId == racePlayerId &&
                    rpt.Type == timeType);

            return racePlayerTime;
        }

        public RacePlayerTime CreateOrUpdate(RacePlayerTime model)
        {
            Guard.Argument(model.RaceId, nameof(model.RaceId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("R-");
            Guard.Argument(model.RacePlayerId, nameof(model.RacePlayerId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("RP-");
            Guard.Argument(model.CategoryId, nameof(model.CategoryId))
                .NotNull().NotEmpty().NotWhiteSpace().StartsWith("C-");

            var rpt = Find(model.RaceId, model.CategoryId, model.RacePlayerId, model.Stage, model.Type);
            if (rpt == null)
            {
                return Create(model.RaceId, model);
            }

            rpt.Time = model.Time;
            return Update(rpt);
        }
    }
}

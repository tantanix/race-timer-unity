using Assets.Tcs.RaceTimer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tcs.RaceTimer.Repository
{
    public class RaceRepository
    {
        private readonly List<Race> _races = new List<Race>();

        public Race CreateRace(string name, DateTime eventDate, int stages)
        {
            var id = Guid.NewGuid();
            return CreateRace(id, name, eventDate, stages);
        }

        public Race CreateRace(Guid id, string name, DateTime eventDate, int stages)
        {
            var race = new Race
            {
                Id = id,
                Name = name,
                EventDate = eventDate,
                Stages = stages
            };

            _races.Add(race);

            return race;
        }

        public Race GetRace(Guid id)
        {
            var race = _races.FirstOrDefault(r => Equals(r.Id, id));
            if (race == null)
                throw new RaceNotFoundException();

            return race;
        }

        public IReadOnlyList<Race> GetAll()
        {
            return _races.AsReadOnly();
        }
    }
}

using Assets.Tcs.RaceTimer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tcs.RaceTimer.Repository
{
    public class RaceRepository
    {
        private readonly List<Race> races = new List<Race>();

        public Race CreateRace(string id, string name, DateTime eventDate, int stages)
        {
            var race = new Race
            {
                Id = id,
                Name = name,
                EventDate = eventDate,
                Stages = stages
            };

            this.races.Add(race);

            return race;
        }

        public Race GetRace(string id)
        {
            var race = this.races.FirstOrDefault(r => r.Id == id);
            if (race == null)
                throw new RaceNotFoundException();

            return race;
        }
    }
}

using Assets.Tcs.RaceTimer.Exceptions;
using Assets.Tcs.RaceTimer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tcs.RaceTimer.Repository
{
    public class TeamRepository
    {
        private readonly List<Team> teams = new List<Team>();

        public Team CreateTeam(Guid id, string name)
        {
            var team = new Team
            {
                Id = id,
                Name = name
            };

            this.teams.Add(team);

            return team;
        }

        public Team Get(string id)
        {
            var team = this.teams.FirstOrDefault(t => Equals(t.Id, id));
            if (team == null)
                throw new TeamNotFoundException();

            return team;
        }
    }
}

using Tcs.RaceTimer.Exceptions;
using Tcs.RaceTimer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tcs.RaceTimer.Repository
{
    public class TeamRepository
    {
        private readonly List<Team> teams = new List<Team>();

        public Team CreateTeam(string id, string name)
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

using Tcs.RaceTimer.Models;
using Tcs.Core.Entity;

namespace Tcs.RaceTimer.Repository
{
    public class TeamRepository : EntityRepository<Team, TeamList>
    {
        public TeamRepository() : base("TeamListIds") { }

        public Team FindByName(string teamName)
        {
            var teams = GetAll();
            foreach (var team in teams)
            {
                if (team.Name == teamName)
                    return team;
            }

            return null;
        }
    }
}

using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Repository;
using Tcs.RaceTimer.Services;
using System;

namespace Tcs.RaceTimer.Import
{
    public class RaceImporter
    {
        private readonly RaceService raceService;

        public RaceImporter(RaceService raceService)
        {
            this.raceService = raceService;
        }

        public void Import()
        {
            //var race = this.raceService.CreateRace(Guid.NewGuid(), "Carcar Enduro Series", DateTime.Now, 4);
            //var player = this.raceService.CreatePlayer(Guid.NewGuid(), "Player 1", "1");
            //var team = this.raceService.CreateTeam(Guid.NewGuid(), "TCS");

            //this.raceService.AddTeamPlayer(race.Id, team.Id, player.Id);

            //this.raceService.AddPlayerTime(race.Id, player.Id, TimeType.Start,
            //    new LogTime { Hours = 0, Minutes = 0, Seconds = 0, Milliseconds = 0 });
        }
    }
}

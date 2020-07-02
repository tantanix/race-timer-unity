using Assets.Tcs.RaceTimer.Models;
using Assets.Tcs.RaceTimer.Repository;
using Assets.Tcs.RaceTimer.Services;
using System;

namespace Assets.Tcs.RaceTimer.Import
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
            var race = this.raceService.CreateRace("1", "Carcar Enduro Series", DateTime.Now, 4);
            var player = this.raceService.CreatePlayer("1", "Player 1", "1");
            var team = this.raceService.CreateTeam("1", "TCS");

            this.raceService.AddTeamPlayer(race.Id, team.Id, player.Id);

            this.raceService.AddPlayerTime(race.Id, player.Id, TimeType.Start,
                new LogTime { Hours = 0, Minutes = 0, Seconds = 0, Milliseconds = 0 });
        }
    }
}

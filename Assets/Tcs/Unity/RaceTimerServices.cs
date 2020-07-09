using Assets.Tcs.RaceTimer.Repository;
using Assets.Tcs.RaceTimer.Services;
using UnityEngine;

public class RaceTimerServices : MonoBehaviour
{
    public RaceService RaceService;
    
    private Race _currentRace;

    void Awake()
    {
        var playerRepo = new PlayerRepository();
        var raceRepo = new RaceRepository();
        var teamRepo = new TeamRepository();

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo);
    }

    public void LoadRace(Race race)
    {
        _currentRace = race;
    }
}

using Assets.Tcs.RaceTimer.Repository;
using Assets.Tcs.RaceTimer.Services;
using UnityEngine;
using UnityEngine.Events;

public class RaceTimerServices : MonoBehaviour
{
    public RaceService RaceService;

    void Awake()
    {
        var playerRepo = new PlayerRepository();
        var raceRepo = new RaceRepository();
        var teamRepo = new TeamRepository();

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo);
    }
}

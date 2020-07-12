using Tcs.RaceTimer.Repository;
using Tcs.RaceTimer.Services;
using UnityEngine;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.Interfaces;

public class RaceTimerServices : MonoBehaviour
{
    private static RaceTimerServices _instance;

    public static RaceTimerServices GetInstance()
    {
        return _instance;
    }

    public IRaceService RaceService;
    
    private Race _currentRace;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        var playerRepo = new PlayerRepository();
        var raceRepo = new RaceRepository();
        var teamRepo = new TeamRepository();
        var racePlayerRepo = new RacePlayerRepository();

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo, racePlayerRepo);
    }

    public void Initialize()
    {
        Debug.Log("RaceTimerServices is initialized");
    }
}

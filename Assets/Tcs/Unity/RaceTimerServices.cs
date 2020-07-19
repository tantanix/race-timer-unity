using Tcs.RaceTimer.Repository;
using Tcs.RaceTimer.Services;
using UnityEngine;
using Tcs.RaceTimer.Interfaces;

public class RaceTimerServices : MonoBehaviour
{
    private static RaceTimerServices _instance;

    public static RaceTimerServices GetInstance()
    {
        return _instance;
    }

    public IRaceService RaceService;
    
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
        var categoryRepo = new CategoryRepository();
        var raceCategoryRepo = new RaceCategoryRepository();
        var racePlayerRepo = new RacePlayerRepository();

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo, categoryRepo, raceCategoryRepo, racePlayerRepo);
    }

    public void Initialize()
    {
        Debug.Log("RaceTimerServices is initialized");
    }
}

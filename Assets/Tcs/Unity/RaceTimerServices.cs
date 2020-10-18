using Assets.Tcs.Unity;
using Tcs.RaceTimer.Repository;
using Tcs.RaceTimer.Services;
using UnityEngine;

public class RaceTimerServices : MonoBehaviour
{
    private static RaceTimerServices _instance;

    public static RaceTimerServices GetInstance()
    {
        return _instance;
    }

    public RaceService RaceService;
    public DialogService DialogService => DialogService.GetInstance();
    public TooltipService TooltipService => TooltipService.GetInstance();
    
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
        var racePlayerTimeRepo = new RacePlayerTimeRepository();

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo, categoryRepo, raceCategoryRepo, racePlayerTimeRepo, racePlayerRepo);
    }

    public void Initialize()
    {
        Debug.Log("RaceTimerServices is initialized");
    }
}

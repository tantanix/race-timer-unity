using Tcs.RaceTimer.Repository;
using Tcs.RaceTimer.Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Tcs.RaceTimer.Models;

public class RaceTimerServices : MonoBehaviour
{
    private static RaceTimerServices _instance;

    public static RaceTimerServices GetInstance()
    {
        return _instance;
    }

    public RaceService RaceService;
    
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

        RaceService = new RaceService(raceRepo, playerRepo, teamRepo);
    }

    public void Initialize()
    {
        Debug.Log("RaceTimerServices is initialized");
    }

    public void LoadRace(Race race)
    {
        
    }
}

using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using UniRx;
using UnityEngine;

public class RacePlayersPanel : MonoBehaviour
{
    public RectTransform RacePlayersContainer;

    private List<GameObject> _racePlayerInstances = new List<GameObject>();

    private void Awake()
    {
        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceLoaded()
            .TakeUntilDestroy(this)
            .Subscribe(LoadPlayerList);

        RaceTimerServices.GetInstance()
            .RaceService
            .OnNewRacePlayer()
            .TakeUntilDestroy(this)
            .Subscribe(CreateRacePlayer);
    }

    private void LoadPlayerList(Race race)
    {
        ClearList();

        var racePlayers = RaceTimerServices.GetInstance().RaceService.GetAllRacePlayers();
        foreach (var racePlayer in racePlayers)
        {
            CreateRacePlayer(racePlayer);
        }
    }

    private void ClearList()
    {
        foreach (var instance in _racePlayerInstances)
        {
            instance.transform.SetParent(null);
            ObjectPool.GetInstance().PoolObject(instance);
        }

        _racePlayerInstances.Clear();
    }

    private void CreateRacePlayer(RacePlayerViewModel racePlayerInfo)
    {
        if (racePlayerInfo == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("RacePlayerEntry", false);
        go.GetComponent<RacePlayerEntry>().SetInfo(racePlayerInfo);

        go.transform.SetParent(RacePlayersContainer, false);
        go.transform.localScale = Vector3.one;

        _racePlayerInstances.Add(go);
    }
}

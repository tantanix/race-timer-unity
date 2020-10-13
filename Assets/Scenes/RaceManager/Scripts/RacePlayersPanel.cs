using System.Collections.Generic;
using System.Linq;
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
            .OnRaceCategoryLoaded()
            .TakeUntilDestroy(this)
            .Subscribe(LoadCategoryPlayers);

        RaceTimerServices.GetInstance()
            .RaceService
            .OnNewRaceCategoryPlayer()
            .TakeUntilDestroy(this)
            .Subscribe(CreateRacePlayer);

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceCategoryDeleted()
            .TakeUntilDestroy(this)
            .Subscribe(RemoveAllCurrentRaceCategoryPlayers);
    }

    private void LoadCategoryPlayers(RaceCategory raceCategory)
    {
        ClearList();

        if (raceCategory == null)
        {
            return;
        }

        var racePlayers = RaceTimerServices.GetInstance().RaceService.GetAllRaceCategoryPlayers();
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
            instance.GetComponent<RacePlayerEntry>().SetInfo(null);
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

    private void RemoveAllCurrentRaceCategoryPlayers(string raceCategoryId)
    {
        ClearList();
    }
}

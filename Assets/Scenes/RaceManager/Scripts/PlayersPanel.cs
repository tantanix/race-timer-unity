using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using UniRx;
using UnityEngine;

public class PlayersPanel : MonoBehaviour
{
    public RectTransform PlayersContainer;
    
    private List<GameObject> _playerInstances = new List<GameObject>();

    void Awake()
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
            .OnNewPlayer()
            .TakeUntilDestroy(this)
            .Subscribe(CreateRacePlayer);
    }

    public void LoadPlayerList(RaceViewModel raceViewModel)
    {
        if (raceViewModel == null)
            return;

        ClearList();

        foreach (var player in raceViewModel.Players)
        {
            CreateRacePlayer(player);
        }
    }

    private void ClearList()
    {
        foreach (var instance in _playerInstances)
        {
            instance.transform.SetParent(null);
            ObjectPool.GetInstance().PoolObject(instance);
        }

        _playerInstances.Clear();
    }

    private void CreateRacePlayer(Player player)
    {
        if (player == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("PlayerEntry", false);
        go.GetComponent<PlayerEntry>().SetInfo(player);

        go.transform.SetParent(PlayersContainer, false);
        go.transform.localScale = Vector3.one;

        _playerInstances.Add(go);
    }

}

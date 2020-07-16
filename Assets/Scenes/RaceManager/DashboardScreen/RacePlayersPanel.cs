﻿using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using UniRx;
using UnityEngine;

public class RacePlayersPanel : MonoBehaviour, IObserver<Player>
{
    public RectTransform RacePlayerContainer;
    
    private List<GameObject> _playerInstances = new List<GameObject>();

    void Start()
    {
        RaceTimerServices.GetInstance()?
            .RaceService
            .OnNewPlayer
            .TakeUntilDestroy(this)
            .Subscribe(CreateRacePlayer);
    }

    public void LoadPlayerList(Race race)
    {
        ClearList();

        var players = RaceTimerServices.GetInstance().RaceService.GetAllPlayers();
        foreach (var player in players)
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

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Player value)
    {
        CreateRacePlayer(value);
    }

    private void CreateRacePlayer(Player player)
    {
        if (player == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("RacePlayerEntry", false);
        go.GetComponent<RacePlayerEntry>().SetInfo(player);

        go.transform.SetParent(RacePlayerContainer, false);
        go.transform.localScale = Vector3.one;

        _playerInstances.Add(go);
    }

}

using System;
using Tcs.RaceTimer.Models;
using UnityEngine;

public class RacePlayersPanel : MonoBehaviour, IObserver<RacePlayerInfo>
{
    public RectTransform RacePlayerContainer;

    void Start()
    {
        RaceTimerServices.GetInstance()?.RaceService.OnNewRacePlayer.Subscribe(this);
    }

    public void LoadPlayerList(Race race)
    {
        var racePlayers = RaceTimerServices.GetInstance().RaceService.GetAllRacePlayers(race.Id);
        foreach (var racePlayer in racePlayers)
        {
            CreateRacePlayer(racePlayer);
        }
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(RacePlayerInfo value)
    {
        throw new NotImplementedException();
    }
    private void CreateRacePlayer(RacePlayerInfo racePlayer)
    {
        var go = ObjectPool.Instance.GetObjectForType("RacePlayerEntry", false);
        go.GetComponent<RacePlayerEntry>().SetInfo(racePlayer);

        go.transform.SetParent(RacePlayerContainer, false);
        go.transform.localScale = Vector3.one;
    }

}

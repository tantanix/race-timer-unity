using Dawn;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using UniRx;
using UnityEngine;

public class PlayerLogTimesList : MonoBehaviour
{
    private List<GameObject> _playerLogTimeInstances = new List<GameObject>();

    private void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnStageSet()
            .TakeUntilDestroy(this)
            .Subscribe(stage => LoadAllPlayerLogTimesByStage(stage));

        RaceTimerServices.GetInstance()
            .RaceService
            .OnNewRacePlayerTime()
            .TakeUntilDestroy(this)
            .Subscribe(racePlayerTime => CreatePlayerLogTimeEntry(racePlayerTime));

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRacePlayerTimeDeleted()
            .TakeUntilDestroy(this)
            .Subscribe(racePlayerTimeId => RemoveRacePlayerTimeFromList(racePlayerTimeId));

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRacePlayerTimeUpdated()
            .TakeUntilDestroy(this)
            .Subscribe(racePlayerTime => UpdateRacePlayerTimeFromList(racePlayerTime));
    }

    private void CreatePlayerLogTimeEntry(RacePlayerTimeViewModel racePlayerTime)
    {
        if (racePlayerTime == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("PlayerLogTimeEntry", false);
        go.transform.SetParent(transform, false);
        go.transform.SetAsFirstSibling();
        go.transform.localScale = Vector3.one;

        _playerLogTimeInstances.Add(go);

        go.GetComponent<PlayerLogTimeEntry>().RacePlayerTime = racePlayerTime;

    }

    private void LoadAllPlayerLogTimesByStage(int stage)
    {
        if (stage <= 0)
            return;

        ClearList();

        var raceId = RaceTimerServices.GetInstance().RaceService.CurrentRace.Id;
        var racePlayerLogTimes = RaceTimerServices.GetInstance().RaceService.GetRacePlayerLogTimes(raceId, stage, TimeType.End);
        foreach (var racePlayerTime in racePlayerLogTimes)
        {
            CreatePlayerLogTimeEntry(racePlayerTime);
        }
    }

    private void RemoveRacePlayerTimeFromList(string racePlayerTimeId)
    {
        if (string.IsNullOrEmpty(racePlayerTimeId))
            return;

        GameObject instanceToRemove = null;
        foreach (var instance in _playerLogTimeInstances)
        {
            if (instance.GetComponent<PlayerLogTimeEntry>().RacePlayerTime.Id == racePlayerTimeId)
            {
                instance.transform.SetParent(null);
                ObjectPool.GetInstance().PoolObject(instance);

                instanceToRemove = instance;
                break;
            }
        }

        if (instanceToRemove)
        {
            _playerLogTimeInstances.Remove(instanceToRemove);
        }
    }

    private void UpdateRacePlayerTimeFromList(RacePlayerTimeViewModel racePlayerTime)
    {
        if (racePlayerTime == null)
            return;

        foreach (var instance in _playerLogTimeInstances)
        {
            var playerLogTimeEntry = instance.GetComponent<PlayerLogTimeEntry>();
            if (playerLogTimeEntry.RacePlayerTime.Id == racePlayerTime.Id)
            {
                playerLogTimeEntry.RacePlayerTime = racePlayerTime;
                break;
            }
        }
    }

    private void ClearList()
    {
        foreach (var instance in _playerLogTimeInstances)
        {
            instance.transform.SetParent(null);
            ObjectPool.GetInstance().PoolObject(instance);
        }

        _playerLogTimeInstances.Clear();
    }
}

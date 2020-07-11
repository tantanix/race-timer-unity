using System;
using TMPro;
using UnityEngine;
using Tcs.RaceTimer.Models;

public class NavPanel : MonoBehaviour, IObserver<Race>
{
    public RaceManagerSceneController Controller;
    public RectTransform ButtonContainer;
    
    void Start()
    {
        RaceTimerServices.GetInstance()?.RaceService.OnNewRace.Subscribe(this);
    }

    public void Initialize()
    {
        var races = RaceTimerServices.GetInstance()?.RaceService.GetAllRaces();
        foreach (var race in races)
        {
            CreateRaceButton(race);
        }
    }

    public void OnCreateRace()
    {
        Controller.ChangeState(RaceManagerSceneController.ScreenState.CreateRace);
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Race value)
    {
        if (value == null)
            return;

        CreateRaceButton(value);
    }

    private void CreateRaceButton(Race race)
    {
        if (race == null)
            return;

        if (string.IsNullOrEmpty(race.Name))
            return;

        var go = ObjectPool.Instance.GetObjectForType("RaceButton", false);
        go.GetComponentInChildren<TMP_Text>().text = race.Name.Substring(0, 1).ToUpperInvariant();
        go.transform.localScale = Vector3.one;
        go.transform.SetParent(ButtonContainer, false);
        go.transform.SetSiblingIndex(1);

        go.GetComponent<RaceButton>().Race = race;
    }
}

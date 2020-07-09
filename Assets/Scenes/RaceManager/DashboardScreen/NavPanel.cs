using Assets.Tcs.RaceTimer.Services;
using System;
using TMPro;
using UnityEngine;

public class NavPanel : MonoBehaviour, IObserver<Race>
{
    public RaceManagerSceneController Controller;
    public RectTransform ButtonContainer;
    
    private RaceService _raceService;

    void Start()
    {
        _raceService = FindObjectOfType<RaceTimerServices>().RaceService;
        _raceService.Subscribe(this);
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
        CreateRaceButton(value);
    }

    private void CreateRaceButton(Race race)
    {
        var go = ObjectPool.Instance.GetObjectForType("RaceButton", false);
        go.GetComponentInChildren<TMP_Text>().text = race.Name.Substring(0, 1).ToUpperInvariant();
        go.transform.localScale = Vector3.one;
        go.transform.SetParent(ButtonContainer, false);
        go.transform.SetSiblingIndex(1);

        go.GetComponent<RaceButton>().Race = race;
    }
}

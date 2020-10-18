using Tcs.RaceTimer.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LogPlayerButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => LogPlayer());
    }

    private void LogPlayer()
    {
        var clock = FindObjectOfType<Clock>();
        var raceService = RaceTimerServices.GetInstance().RaceService;
        var raceId = raceService.CurrentRace.Id;
        var stage = raceService.CurrentStage;
        raceService.CreateRacePlayerTime(raceId, stage, clock.CurrentTime, TimeType.End);
    }
}

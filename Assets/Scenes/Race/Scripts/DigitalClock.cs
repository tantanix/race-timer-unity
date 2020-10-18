using Tcs.RaceTimer.Services;
using TMPro;
using UniRx;
using UnityEngine;

public class DigitalClock : MonoBehaviour
{
    public TMP_Text Text;

    private Clock _clock;
    private RaceService _raceService;
    private int _stage;

    void Awake()
    {
        _clock = FindObjectOfType<Clock>();
    }

    private void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnStageSet()
            .TakeUntilDestroy(this)
            .Subscribe(stage => _stage = stage);
        
    }

    void Update()
    {
        if (_stage > 0)
        {
            Text.text = GetCurrentTime();
        }
    }

    private string GetCurrentTime()
    {
        var time = _clock.CurrentTime;
        var hours = time.Hours > 12 ? time.Hours - 12 : time.Hours;
        var timeOfDay = time.Hours > 12 ? "PM" : "AM";
        return $"{hours:00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:0000} {timeOfDay}";
    }
}

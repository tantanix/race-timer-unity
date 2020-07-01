using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalClock : MonoBehaviour
{
    public Text Text;

    private Clock _clock;

    void Awake()
    {
        _clock = FindObjectOfType<Clock>();
    }

    void Update()
    {
        Text.text = GetCurrentTime();
    }

    private string GetCurrentTime()
    {
        var hours = _clock.Hours > 12 ? _clock.Hours - 12 : _clock.Hours;
        var timeOfDay = _clock.Hours > 12 ? "PM" : "AM";
        return $"{hours:00}:{_clock.Minutes:00}:{_clock.Seconds:00}:{_clock.Milliseconds:0000} {timeOfDay}";
    }
}

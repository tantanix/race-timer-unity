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
        var time = _clock.CurrentTime;
        var hours = time.Hours > 12 ? time.Hours - 12 : time.Hours;
        var timeOfDay = time.Hours > 12 ? "PM" : "AM";
        return $"{hours:00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:0000} {timeOfDay}";
    }
}

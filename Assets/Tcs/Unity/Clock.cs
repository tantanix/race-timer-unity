using Assets.Tcs.RaceTimer.Models;
using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public LogTime CurrentTime;

    void Update()
    {
        var date = DateTime.Now;

        CurrentTime.Hours = date.Hour;
        CurrentTime.Minutes = date.Minute;
        CurrentTime.Seconds = date.Second;
        CurrentTime.Milliseconds = date.Millisecond;
    }
}

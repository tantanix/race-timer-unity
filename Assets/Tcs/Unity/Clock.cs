using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public int Hours, Minutes, Seconds, Milliseconds;

    void Update()
    {
        var date = DateTime.Now;
        this.Hours = date.Hour;
        this.Minutes = date.Minute;
        this.Seconds = date.Second;
        this.Milliseconds = date.Millisecond;
    }
}

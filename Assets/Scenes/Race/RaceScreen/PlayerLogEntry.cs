using Assets.Tcs.RaceTimer.Models;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogEntry : MonoBehaviour
{
    public Text TimeLogText;
    public Text PlayerNoText;
    public InputField PlayerNoInput;

    void Awake()
    {
        PlayerNoText.gameObject.SetActive(false);
        PlayerNoInput.onEndEdit.AddListener(OnFinishEditingPlayerNo);
    }

    void OnFinishEditingPlayerNo(string value)
    {
        if (int.TryParse(value, out var riderNo))
        {
            // TODO: Check rider no exists
            PlayerNoText.gameObject.SetActive(true);
            PlayerNoText.text = value;

            PlayerNoInput.gameObject.SetActive(false);
        }
    }

    public void SetLogTime(LogTime time)
    {
        TimeLogText.text = GetCurrentTime(time);
    }

    private string GetCurrentTime(LogTime time)
    {
        var adjustedHours = time.Hours > 12 ? time.Hours - 12 : time.Hours;
        var timeOfDay = time.Hours > 12 ? "PM" : "AM";
        return $"{adjustedHours:00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:0000} {timeOfDay}";
    }
}

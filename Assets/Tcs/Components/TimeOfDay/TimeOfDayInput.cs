using System;
using System.Collections.Generic;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;

public class TimeOfDayInput : MonoBehaviour
{
    public const string HourOption = "HH";
    public const string MinuteOption = "MM";

    public TMP_Dropdown HourDropdown;
    public TMP_Dropdown MinuteDropdown;
    public TMP_Dropdown AmPmDropdown;

    private List<string> _hours = new List<string>();
    private List<string> _minutes = new List<string>();
    private List<string> _ampm = new List<string>();

    private void Start()
    {
        HourDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectHour(index));

        MinuteDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectMinute(index));

        InitializeList();
    }

    private void InitializeList()
    {
        // Hours
        _hours.Clear();
        _hours.Add(HourOption);

        for (var i = 0; i < 12; i++)
        {
            _hours.Add($"{i:D2}");
        }

        HourDropdown.ClearOptions();
        HourDropdown.AddOptions(_hours);

        // Minutes
        _minutes.Clear();
        _minutes.Add(MinuteOption);

        for (var i = 0; i < 60; i++)
        {
            _minutes.Add($"{i:D2}");
        }

        MinuteDropdown.ClearOptions();
        MinuteDropdown.AddOptions(_minutes);

        // AM PM
        AmPmDropdown.ClearOptions();
        AmPmDropdown.AddOptions(new List<string> { "AM", "PM" });
        AmPmDropdown.SetValueWithoutNotify(0);
        
    }

    private void SelectHour(int index)
    {
        
    }

    private void SelectMinute(int index)
    {

    }
}

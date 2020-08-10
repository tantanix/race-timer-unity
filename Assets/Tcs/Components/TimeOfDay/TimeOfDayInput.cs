using System;
using System.Collections.Generic;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class TimeOfDayInput : MonoBehaviour
{
    [System.Serializable]
    public class TimeOfDayEvent : UnityEvent<TimeSpan>
    {
        public TimeOfDayEvent() { }
    }

    public const string AM = "AM";
    public const string PM = "PM";

    public const string HourOption = "HH";
    public const string MinuteOption = "MM";

    public TMP_Dropdown HourDropdown;
    public TMP_Dropdown MinuteDropdown;
    public TMP_Dropdown AmPmDropdown;

    public TimeOfDayEvent OnValueChanged = new TimeOfDayEvent();
    public TimeSpan Value => GetCurrentTimespan();

    private List<string> _hours = new List<string>();
    private List<string> _minutes = new List<string>();
    private List<string> _ampm = new List<string>();
    
    private int _selectedHour;
    private int _selectedMinute;
    private string _selectedAmPm;

    private void Start()
    {
        Initialize();

        HourDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectHour(index));

        MinuteDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectMinute(index));

        AmPmDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectAmPm(index));
    }

    public void Initialize()
    {
        // Hours
        _hours.Clear();
        _hours.Add(HourOption);

        for (var i = 1; i <= 12; i++)
        {
            _hours.Add($"{i:D1}");
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
        _ampm.Clear();
        _ampm.Add(AM);
        _ampm.Add(PM);

        AmPmDropdown.ClearOptions();
        AmPmDropdown.AddOptions(_ampm);
        AmPmDropdown.SetValueWithoutNotify(0);
        _selectedAmPm = AM;
    }

    private void SelectHour(int index)
    {
        var selectedHour = _hours[index];
        if (int.TryParse(selectedHour, out int hour))
        {
            _selectedHour = hour;
            OnValueChanged.Invoke(GetCurrentTimespan());
        }
        else if (selectedHour == HourOption)
        {
            _selectedHour = -1;
        }
    }

    private void SelectMinute(int index)
    {
        var selectedMinute = _minutes[index];
        if (int.TryParse(selectedMinute, out int minute))
        {
            _selectedMinute = minute;
            OnValueChanged.Invoke(GetCurrentTimespan());
        }
        else if (selectedMinute == MinuteOption)
        {
            _selectedMinute = -1;
        }
    }

    private void SelectAmPm(int index)
    {
        _selectedAmPm = _ampm[index];
        OnValueChanged.Invoke(GetCurrentTimespan());
    }

    private TimeSpan GetCurrentTimespan()
    {
        if (_selectedHour < 0 || _selectedMinute < 0)
            return new TimeSpan();

        var hours = _selectedAmPm == AM ? _selectedHour : _selectedHour + 12;
        
        return new TimeSpan(hours, _selectedMinute, 0);
    }
}

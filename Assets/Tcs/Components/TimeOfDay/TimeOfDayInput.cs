using System;
using System.Collections.Generic;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeOfDayInput : MonoBehaviour
{
    [System.Serializable]
    public class TimeOfDayEvent : UnityEvent<TimeSpan?>
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
    public Button ConfirmButton;

    public TimeOfDayEvent OnValueChanged = new TimeOfDayEvent();
    public TimeSpan? CurrentTimeSpan;

    private List<string> _hours = new List<string>();
    private List<string> _minutes = new List<string>();
    private List<string> _ampm = new List<string>();
    
    private int? _selectedHour;
    private int? _selectedMinute;
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

        ConfirmButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ConfirmChange());
    }

    public void Initialize()
    {
        CurrentTimeSpan = null;

        // Hours
        _hours.Clear();
        _hours.Add(HourOption);

        for (var i = 1; i <= 12; i++)
        {
            _hours.Add($"{i:D1}");
        }

        HourDropdown.ClearOptions();
        HourDropdown.AddOptions(_hours);
        _selectedHour = null;

        // Minutes
        _minutes.Clear();
        _minutes.Add(MinuteOption);

        for (var i = 0; i < 60; i++)
        {
            _minutes.Add($"{i:D2}");
        }

        MinuteDropdown.ClearOptions();
        MinuteDropdown.AddOptions(_minutes);
        MinuteDropdown.SetValueWithoutNotify(1);
        _selectedMinute = 0;

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
        _selectedHour = null;
        if (int.TryParse(selectedHour, out int hour))
        {
            _selectedHour = hour;
        }
    }

    private void SelectMinute(int index)
    {
        var selectedMinute = _minutes[index];
        _selectedMinute = null;
        if (int.TryParse(selectedMinute, out int minute))
        {
            _selectedMinute = minute;
        }
    }

    private void SelectAmPm(int index)
    {
        _selectedAmPm = _ampm[index];
    }

    private void ConfirmChange()
    {
        CurrentTimeSpan = GetCurrentTimespan();
        OnValueChanged.Invoke(CurrentTimeSpan);
    }

    private TimeSpan? GetCurrentTimespan()
    {
        if (!_selectedHour.HasValue || !_selectedMinute.HasValue)
            return null;

        var hours = _selectedHour == 12 && _selectedAmPm == AM ? _selectedHour - 12 : _selectedHour;
        if (_selectedHour == 12)
        {
            if (_selectedAmPm == AM)
                hours = 0;
            else
                hours = _selectedHour;
        }
        else
        {
            if (_selectedAmPm == AM)
                hours = _selectedHour;
            else
                hours += 12;
        }
        
        return new TimeSpan(hours.Value, _selectedMinute.Value, 0);
    }
}

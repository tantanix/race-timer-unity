using System;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MatTimeOfDay : MonoBehaviour
{
    [System.Serializable]
    public class TimeOfDayEvent : UnityEvent<TimeSpan>
    {
        public TimeOfDayEvent() { }
    }

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TimeOfDayEvent OnValueChanged = new TimeOfDayEvent();
    public Transform InputTransform;
    public Button Button;
    public TimeOfDayInput TimeOfDayInput;
    public TMP_Text PlaceholderText;
    public TMP_Text LabelText;
    public TMP_Text ErrorText;
    public TMP_Text InnerText;
    public Image Line;
    public bool IsRequired;

    private Color32 _lineDefaultColor;
    private Color32 _labelTextDefaultColor;
    private TimeSpan? _currentTime;

    private void Awake()
    {
        _lineDefaultColor = Line.color;
        _labelTextDefaultColor = LabelText.color;

        Initialize();

        Button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowSelection());

        TimeOfDayInput
            .OnValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(timespan => SaveTimeSpan(timespan));
    }
    
    public void Initialize()
    {
        _currentTime = null;

        TimeOfDayInput.Initialize();
        TimeOfDayInput.gameObject.SetActive(false);
        InputTransform.gameObject.SetActive(true);

        ErrorText.gameObject.SetActive(false);
        LabelText.gameObject.SetActive(false);

        InnerText.text = "";
        PlaceholderText.gameObject.SetActive(true);
        InnerText.gameObject.SetActive(false);

        LabelText.color = _labelTextDefaultColor;
        Line.color = _lineDefaultColor;
    }

    public void Validate()
    {
        if (TimeOfDayInput.isActiveAndEnabled)
        {
            InputTransform.gameObject.SetActive(true);
            TimeOfDayInput.gameObject.SetActive(false);
        }

        var requiredPassed = _currentTime.HasValue;
        if (IsRequired)
        {
            LabelText.gameObject.SetActive(requiredPassed);
            PlaceholderText.gameObject.SetActive(!requiredPassed);

            Line.color = requiredPassed ? _lineDefaultColor : InvalidBgColor;
            ErrorText.gameObject.SetActive(!requiredPassed);
        }
        else
        {
            LabelText.gameObject.SetActive(requiredPassed);
            PlaceholderText.gameObject.SetActive(!requiredPassed);
            ErrorText.gameObject.SetActive(false);
        }
    }

    private void SaveTimeSpan(TimeSpan? timeSpan)
    {
        _currentTime = timeSpan;
        if (_currentTime.HasValue)
        {
            var timeOfDay = _currentTime.Value.Hours >= 12 ? "PM" : "AM";

            int hours;
            if (_currentTime.Value.Hours == 0)
            {
                hours = 12;
            }
            else
            {
                hours = _currentTime.Value.Hours > 12 ? _currentTime.Value.Hours - 12 : _currentTime.Value.Hours;
            }

            InnerText.text = $"{hours}:{_currentTime.Value.Minutes:D2} {timeOfDay}";
            InnerText.gameObject.SetActive(true);
        }
        else
        {
            InnerText.text = "";
            InnerText.gameObject.SetActive(false);
        }

        InputTransform.gameObject.SetActive(true);
        TimeOfDayInput.gameObject.SetActive(false);

        Validate();
    }

    private void ShowSelection()
    {
        InputTransform.gameObject.SetActive(false);
        TimeOfDayInput.gameObject.SetActive(true);
    }
}

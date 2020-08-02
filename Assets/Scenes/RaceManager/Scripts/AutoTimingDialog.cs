using System;
using System.Collections.Generic;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AutoTimingDialog : MonoBehaviour
{
    public const string SelectStageOption = "Select a stage";
    public const string SelectStartTimeOption = "Select start time";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_Dropdown StageDropdown;
    public TimeOfDayInput TimeOfDay;
    public TMP_InputField RiderIntervalSecondsInput;
    public TMP_InputField CategoryIntervalSecondsInput;
    public Button SetButton;
    public Button CloseButton;

    private List<string> _stages = new List<string>();
    private List<string> _timeSlots = new List<string>();
    private int _selectedStage;

    private void Start()
    {
        StageDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectStage(index));

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());

        SetButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetTiming());
    }

    public void Initialize()
    {
        InitializeStageDropdown();
    }

    private void InitializeStageDropdown()
    {
        _stages.Clear();
        _stages.Add(SelectStageOption);

        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var i = 1;
        while (i <= race.Stages)
        {
            _stages.Add($"Stage {i}");
            i++;
        }

        StageDropdown.ClearOptions();
        StageDropdown.AddOptions(_stages);
    }

    private void SetTiming()
    {
        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;

        var isStageValid = _selectedStage > 0 && _selectedStage <= race.Stages;
        var isRiderIntervalValid = float.TryParse(RiderIntervalSecondsInput.text, out float riderInterval);
        var isCategoryIntervalValid = float.TryParse(CategoryIntervalSecondsInput.text, out float categoryInterval);

        StageDropdown.GetComponent<Image>().color = isStageValid ? ValidBgColor : InvalidBgColor;
        RiderIntervalSecondsInput.GetComponent<Image>().color = isRiderIntervalValid ? ValidBgColor : InvalidBgColor;
        CategoryIntervalSecondsInput.GetComponent<Image>().color = isCategoryIntervalValid ? ValidBgColor : InvalidBgColor;

        if (!isStageValid || !isRiderIntervalValid || !isCategoryIntervalValid)
            return;

        try
        {
            RaceTimerServices.GetInstance()
                .RaceService
                .SetAutoTiming(_selectedStage, DateTime.Now, riderInterval, categoryInterval);
        }
        catch (Exception ex)
        {
            throw new UnityException("Failed to set auto timing for players", ex);
        }
    }

    private void SelectStage(int index)
    {
        var selectedStage = _stages[index];
        if (int.TryParse(selectedStage.Replace("Stage ", ""), out int stage)) {
            var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;

            if (stage > 0 && stage <= race.Stages)
            {
                _selectedStage = stage;
                return;
            }
        }
        else if (selectedStage == SelectStageOption)
        {
            _selectedStage = 0;
            return;
        }

        throw new UnityException("Not a valid stage"); 
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }
}

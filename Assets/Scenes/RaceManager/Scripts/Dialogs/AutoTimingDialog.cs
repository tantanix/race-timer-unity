using System;
using System.Collections.Generic;
using Tcs.Core.Validators;
using Tcs.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AutoTimingDialog : MonoBehaviour
{
    public const string SelectStageOption = "Select a starting stage *";
    public const string SelectStageAfterBreakOption = "Select a starting stage after break (optional)";
    public const string SelectStartTimeOption = "Select start time";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 ValidTimeOfDayBgColor = AppColors.FormTimeOfDayFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public MatTimeOfDay StartRaceTimeOfDay;
    public MatInput RiderIntervalInSecondsInput;
    public MatInput CategoryIntervalSecondsInput;
    public MatInput StageIntervalSecondsInput;

    public Toggle AddBreakToggle;
    public MatTimeOfDay AfterBreakTimeOfDay;
    public MatDropdown StageAfterBreakDropdown;

    public Button SetButton;
    public Button CloseButton;

    private List<string> _stagesAfterBreak = new List<string>();
    
    private int? _selectedStageAfterBreak;
    private TimeSpan? _selectedStartRaceTimeOfDay;
    private TimeSpan? _selectedStartRaceAfterBreakTimeOfDay;
    
    private void Awake()
    {
        StartRaceTimeOfDay
            .OnValueChanged
            .AsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(SetStartRaceTimeOfDay);

        StageAfterBreakDropdown
            .InnerDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(SelectStageAfterBreak);

        AfterBreakTimeOfDay
            .OnValueChanged
            .AsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(SetStartRaceAfterBreakTimeOfDay);

        AddBreakToggle
            .OnValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(ShowBreakOptions);

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());

        SetButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetTiming());

        StartRaceTimeOfDay.AddValidator(timeOfDay => timeOfDay.CurrentTimeSpan.HasValue, "Start race time is required");
        RiderIntervalInSecondsInput.AddValidator(Validators.RequiredAndValidNumberInputField, "Rider interval is required");
        CategoryIntervalSecondsInput.AddValidator(Validators.RequiredAndValidNumberInputField, "Category interval is required");
        StageIntervalSecondsInput.AddValidator(Validators.RequiredAndValidNumberInputField, "Stage interval is required");

        AfterBreakTimeOfDay.AddValidator(timeOfDay => timeOfDay.CurrentTimeSpan.HasValue, "Start race time is required if you add a break");
        StageAfterBreakDropdown.AddValidator(Validators.RequiredDropdown, "Stage is required if you add a break");
    }

    public void Initialize()
    {
        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;

        StartRaceTimeOfDay.Initialize();
        RiderIntervalInSecondsInput.Initialize();
        CategoryIntervalSecondsInput.Initialize();
        StageIntervalSecondsInput.Initialize();

        AddBreakToggle.isOn = false;
        InitializeStageDropdowns(race.Stages, _stagesAfterBreak, SelectStageAfterBreakOption, StageAfterBreakDropdown);
        AfterBreakTimeOfDay.Initialize();
        
        _selectedStageAfterBreak = null;
        _selectedStartRaceAfterBreakTimeOfDay = null;
        _selectedStartRaceTimeOfDay = null;

        AddBreakToggle.isOn = false;
    }

    private void InitializeStageDropdowns(int stagesCount, List<string> stages, string defaultOption, MatDropdown matDropdown)
    {
        stages.Clear();
        stages.Add(defaultOption);

        var i = 1;
        while (i <= stagesCount)
        {
            stages.Add($"Stage {i}");
            i++;
        }

        matDropdown.InnerDropdown.ClearOptions();
        matDropdown.InnerDropdown.AddOptions(stages);
    }

    private void SetTiming()
    {
        var hasBreak = AddBreakToggle.isOn;
        var isStartRaceTimeValid = StartRaceTimeOfDay.Validate();
        var isRiderIntervalValid = RiderIntervalInSecondsInput.Validate();
        var isCategoryIntervalValid = CategoryIntervalSecondsInput.Validate();
        var isStageIntervalValid = StageIntervalSecondsInput.Validate();

        bool isStartTimeAfterBreakValid = true;
        bool isStageAfterBreakValid = true;

        if (hasBreak)
        {
            isStartTimeAfterBreakValid = AfterBreakTimeOfDay.Validate();
            isStageAfterBreakValid = StageAfterBreakDropdown.Validate();
        }

        if (!isStartRaceTimeValid || !isRiderIntervalValid || !isCategoryIntervalValid || !isStageIntervalValid || !isStartTimeAfterBreakValid || !isStageAfterBreakValid)
            return;

        try
        {
            RaceTimerServices.GetInstance()
                .RaceService
                .SetAutoTiming(
                    _selectedStartRaceTimeOfDay.Value,
                    int.Parse(RiderIntervalInSecondsInput.text),
                    int.Parse(CategoryIntervalSecondsInput.text),
                    int.Parse(StageIntervalSecondsInput.text),
                    hasBreak,
                    _selectedStartRaceAfterBreakTimeOfDay,
                    _selectedStageAfterBreak);
        }
        catch (Exception ex)
        {
            throw new UnityException("Failed to set auto timing for players", ex);
        }
    }

    private void SelectStageAfterBreak(int index)
    {
        if (_stagesAfterBreak.Count <= 0)
            return;

        var selectedStage = _stagesAfterBreak[index];
        if (int.TryParse(selectedStage.Replace("Stage ", ""), out int stage))
        {
            var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;

            if (stage > 0 && stage <= race.Stages)
            {
                _selectedStageAfterBreak = stage;
                return;
            }
        }
        else if (selectedStage == SelectStageAfterBreakOption)
        {
            _selectedStageAfterBreak = null;
            return;
        }

        throw new UnityException("Not a valid stage");
    }

    private void SetStartRaceTimeOfDay(TimeSpan? timeSpan)
    {
        _selectedStartRaceTimeOfDay = timeSpan;
    }

    private void SetStartRaceAfterBreakTimeOfDay(TimeSpan? timeSpan)
    {
        _selectedStartRaceAfterBreakTimeOfDay = timeSpan;
    }

    private void ShowBreakOptions(bool isOn)
    {
        AfterBreakTimeOfDay.gameObject.SetActive(isOn);
        StageAfterBreakDropdown.gameObject.SetActive(isOn);
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }
}

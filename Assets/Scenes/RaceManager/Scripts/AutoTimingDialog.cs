using System;
using System.Collections.Generic;
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
    
    private void Start()
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
        var isStartRaceTimeValid = _selectedStartRaceTimeOfDay.HasValue;
        var isRiderIntervalValid = int.TryParse(RiderIntervalInSecondsInput.text, out int riderInterval);
        var isCategoryIntervalValid = int.TryParse(CategoryIntervalSecondsInput.text, out int categoryInterval);
        var isStageIntervalValid = int.TryParse(StageIntervalSecondsInput.text, out int stageInterval);

        if (!isStartRaceTimeValid) StartRaceTimeOfDay.Validate();
        if (!isRiderIntervalValid) RiderIntervalInSecondsInput.Validate();
        if (!isCategoryIntervalValid) CategoryIntervalSecondsInput.Validate();
        if (!isStageIntervalValid) StageIntervalSecondsInput.Validate();
        
        bool isStartTimeAfterBreakValid = true;
        bool isStageAfterBreakValid = true;

        if (hasBreak)
        {
            isStartTimeAfterBreakValid = _selectedStartRaceAfterBreakTimeOfDay.HasValue;
            isStageAfterBreakValid = _selectedStageAfterBreak.HasValue;

            // TODO: Fix me
            //if (!isStartTimeAfterBreakValid) AfterBreakTimeOfDay.Validate();
            //StageAfterBreakDropdown.GetComponent<Image>().color = isStageAfterBreakValid ? ValidBgColor : InvalidBgColor;
        }

        if (!isStartRaceTimeValid || !isRiderIntervalValid || !isCategoryIntervalValid || !isStageIntervalValid || !isStartTimeAfterBreakValid || !isStageAfterBreakValid)
            return;

        try
        {
            RaceTimerServices.GetInstance()
                .RaceService
                .SetAutoTiming(
                    _selectedStartRaceTimeOfDay.Value,
                    riderInterval,
                    categoryInterval,
                    stageInterval,
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

    private void SetStartRaceTimeOfDay(TimeSpan timeSpan)
    {
        if (timeSpan.Ticks > 0)
            _selectedStartRaceTimeOfDay = timeSpan;
        else
            _selectedStartRaceTimeOfDay = null;
    }

    private void SetStartRaceAfterBreakTimeOfDay(TimeSpan timeSpan)
    {
        if (timeSpan.Ticks > 0)
            _selectedStartRaceAfterBreakTimeOfDay = timeSpan;
        else
            _selectedStartRaceAfterBreakTimeOfDay = null;
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

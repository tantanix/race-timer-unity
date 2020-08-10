using System;
using System.Collections.Generic;
using Tcs.Unity;
using TMPro;
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

    public TimeOfDayInput StartRaceTimeOfDay;
    public TMP_InputField RiderIntervalSecondsInput;
    public TMP_InputField CategoryIntervalSecondsInput;
    public TMP_InputField StageIntervalSecondsInput;

    public Toggle AddBreakToggle;
    public TimeOfDayInput AfterBreakTimeOfDay;
    public TMP_Dropdown StageAfterBreakDropdown;

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
        RiderIntervalSecondsInput.text = "";
        CategoryIntervalSecondsInput.text = "";

        AddBreakToggle.isOn = false;
        InitializeStageDropdowns(race.Stages, _stagesAfterBreak, SelectStageAfterBreakOption, StageAfterBreakDropdown);
        AfterBreakTimeOfDay.Initialize();
        
        _selectedStageAfterBreak = null;
        _selectedStartRaceAfterBreakTimeOfDay = null;
        _selectedStartRaceTimeOfDay = null;

        AddBreakToggle.isOn = false;
    }

    private void InitializeStageDropdowns(int stagesCount, List<string> stages, string defaultOption, TMP_Dropdown dropdown)
    {
        stages.Clear();
        stages.Add(defaultOption);

        var i = 1;
        while (i <= stagesCount)
        {
            stages.Add($"Stage {i}");
            i++;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(stages);
    }

    private void SetTiming()
    {
        var hasBreak = AddBreakToggle.isOn;
        var isStartRaceTimeValid = _selectedStartRaceTimeOfDay.HasValue;
        var isRiderIntervalValid = int.TryParse(RiderIntervalSecondsInput.text, out int riderInterval);
        var isCategoryIntervalValid = int.TryParse(CategoryIntervalSecondsInput.text, out int categoryInterval);
        var isStageIntervalValid = int.TryParse(StageIntervalSecondsInput.text, out int stageInterval);
        
        StartRaceTimeOfDay.GetComponent<Image>().color = isStartRaceTimeValid ? ValidTimeOfDayBgColor : InvalidBgColor;
        RiderIntervalSecondsInput.GetComponent<Image>().color = isRiderIntervalValid ? ValidBgColor : InvalidBgColor;
        CategoryIntervalSecondsInput.GetComponent<Image>().color = isCategoryIntervalValid ? ValidBgColor : InvalidBgColor;
        StageIntervalSecondsInput.GetComponent<Image>().color = isStageIntervalValid ? ValidBgColor : InvalidBgColor;

        bool isStartTimeAfterBreakValid = true;
        bool isStageAfterBreakValid = true;

        if (hasBreak)
        {
            isStartTimeAfterBreakValid = _selectedStartRaceAfterBreakTimeOfDay.HasValue;
            isStageAfterBreakValid = _selectedStageAfterBreak.HasValue;

            AfterBreakTimeOfDay.GetComponent<Image>().color = isStartTimeAfterBreakValid ? ValidTimeOfDayBgColor : InvalidBgColor;
            StageAfterBreakDropdown.GetComponent<Image>().color = isStageAfterBreakValid ? ValidBgColor : InvalidBgColor;
        }

        if (!isRiderIntervalValid || !isCategoryIntervalValid || !isStageIntervalValid || !isStartTimeAfterBreakValid || !isStageAfterBreakValid)
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

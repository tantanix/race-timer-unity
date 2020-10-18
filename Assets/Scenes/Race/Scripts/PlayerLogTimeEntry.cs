﻿using System;
using System.Linq;
using Tcs.RaceTimer.Enums;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogTimeEntry : MonoBehaviour
{
    public Color ColorInvalid;
    public Color ColorValid;

    public TMP_Text TimeText;
    public TMP_Text PlayerNoText;
    public TMP_InputField PlayerNoInput;
    public Button DeleteButton;
    public Button EditButton;

    public RacePlayerTimeViewModel RacePlayerTime
    {
        get { return _racePlayerTime; }
        set
        {
            _racePlayerTime = value;

            SetLogTime(_racePlayerTime.Time);
            SetPlayerNo(_racePlayerTime.PlayerNo);
            UpdateStatus(_racePlayerTime.Status);
        }
    }

    private RacePlayerTimeViewModel _racePlayerTime;

    private void Awake()
    {
        PlayerNoInput.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeleteButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowConfirmationDialog());

        EditButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => EditPlayerLog());

        PlayerNoInput
            .OnEndEditAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(playerNo => OnFinishEditingPlayerNo(playerNo));
    }

    private void OnFinishEditingPlayerNo(string value)
    {
        RaceTimerServices.GetInstance()
            .RaceService
            .UpdateRacePlayerTimePlayerNo(RacePlayerTime.Id, value);
    }

    private void SetLogTime(LogTime? time)
    {
        if (time.HasValue)
        {
            TimeText.text = GetCurrentTime(time.Value);
        }
    }

    private void SetPlayerNo(int? playerNo)
    {
        if (playerNo.HasValue)
        {
            PlayerNoText.gameObject.SetActive(true);
            PlayerNoText.text = $"Rider #{playerNo.Value}";
        } 
        else
        {
            PlayerNoText.gameObject.SetActive(false);
        }
    }

    private void UpdateStatus(PlayerLogTimeStatus? status)
    {
        if (status.HasValue)
        {
            var image = GetComponent<Image>();
            var isInvalid = (new[]
            {
                PlayerLogTimeStatus.Duplicate,
                PlayerLogTimeStatus.InvalidPlayerNo,
                PlayerLogTimeStatus.PlayerNonExistent
            })
            .Contains(status.Value);

            if (isInvalid)
            {
                image.color = ColorInvalid;
            }
            else if (status == PlayerLogTimeStatus.Valid)
            {
                image.color = ColorValid;
            }
        }
    }

    private string GetCurrentTime(LogTime time)
    {
        var adjustedHours = time.Hours > 12 ? time.Hours - 12 : time.Hours;
        var timeOfDay = time.Hours > 12 ? "PM" : "AM";
        return $"{adjustedHours:00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:0000} {timeOfDay}";
    }

    private void ShowConfirmationDialog()
    {
        var raceService = RaceTimerServices.GetInstance().RaceService;

        var go = ObjectPool.GetInstance().GetObjectForType("ConfirmationDialog", true);
        go.GetComponent<ConfirmationDialog>().Initialize("Confirm Delete", $"Are you sure you want to delete this\nplayer log?\n\nWarning:  you cannot undo this.");

        var dialog = DialogService.GetInstance().Show(go);
        dialog
            .AfterClosed()
            .TakeUntilDestroy(dialog)
            .Subscribe(data =>
            {
                var confirm = Convert.ToBoolean(data);
                if (confirm)
                {
                    ConfirmDeletePlayerLog();
                }
            });
    }

    private void ConfirmDeletePlayerLog()
    {
        var raceService = RaceTimerServices.GetInstance().RaceService;
        RaceTimerServices.GetInstance().RaceService.DeletePlayerLogTime(raceService.CurrentRace.Id, RacePlayerTime.Id);
    }

    private void EditPlayerLog()
    {

    }
}

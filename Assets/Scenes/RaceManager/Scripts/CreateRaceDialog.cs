﻿using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CreateRaceDialog : MonoBehaviour
{
    public Color32 ValidBgColor;
    public Color32 RequiredBgColor;
    public TMP_InputField RaceNameInput;
    public TMP_InputField NumberOfStagesInput;
    public TMP_InputField EventDateInput;
    public TMP_InputField LocationInput;
    public Button CreateRaceButton;
    public Button CloseButton;

    void Start()
    {
        CreateRaceButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnCreateRace());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnClose());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (RaceNameInput.isFocused)
                NumberOfStagesInput.Select();
            else if (NumberOfStagesInput.isFocused)
                EventDateInput.Select();
            else if (EventDateInput.isFocused)
                LocationInput.Select();
            else if (LocationInput.isFocused)
                CreateRaceButton.Select();
            else
                RaceNameInput.Select();
        }
    }

    public void OnCreateRace()
    {
        var numberOfStages = 0;
        var eventDate = DateTime.Now;
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        var styles = DateTimeStyles.None;

        var isRaceNameValid = RaceNameInput.text.Length > 0;
        var isNumberOfStagesValid = NumberOfStagesInput.text.Length > 0 && int.TryParse(NumberOfStagesInput.text, out numberOfStages);
        var isEventDateValid = EventDateInput.text.Length > 0 && DateTime.TryParse(EventDateInput.text, culture, styles, out eventDate);
        
        RaceNameInput.GetComponent<Image>().color = isRaceNameValid ? ValidBgColor : RequiredBgColor;
        NumberOfStagesInput.GetComponent<Image>().color = isNumberOfStagesValid ? ValidBgColor : RequiredBgColor;
        EventDateInput.GetComponent<Image>().color = isEventDateValid ? ValidBgColor : RequiredBgColor;

        if (!isRaceNameValid || !isNumberOfStagesValid || !isEventDateValid)
            return;

        try
        {
            var race = RaceTimerServices.GetInstance().RaceService.CreateRace(RaceNameInput.text, eventDate.Ticks, numberOfStages, LocationInput.text);
            if (race != null)
                DialogService.GetInstance().Close(gameObject, true);
            else
                throw new Exception("Failed to create race");
        } 
        catch (Exception ex)
        {
            throw new UnityException(ex.Message);
        }
    }

    private void OnClose()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

}
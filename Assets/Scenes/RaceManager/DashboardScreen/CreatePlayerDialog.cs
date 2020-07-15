using Assets.Tcs.Unity;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerDialog : AppBase
{
    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_InputField PlayerNameInput;
    public TMP_InputField TeamNameInput;
    public TMP_InputField AgeInput;
    public TMP_InputField EmailInput;
    public TMP_Dropdown CategoryDropdown;
    public Button CreatePlayerButton;

    private bool isCategoryDropdownFocused = false;

    void Start()
    {
        CreatePlayerButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnCreatePlayer());

        // TODO: Make this list dynamic
        var categories = new List<string>
        {
            "Open elite",
            "40 and above",
            "30-39",
            "20-29",
            "19 and below",
            "Feminino",
            "Wildcard",
            "Curious"
        };
        CategoryDropdown.AddOptions(categories);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (PlayerNameInput.isFocused)
                TeamNameInput.Select();
            else if (TeamNameInput.isFocused)
                AgeInput.Select();
            else if (AgeInput.isFocused)
                EmailInput.Select();
            else if (EmailInput.isFocused)
            {
                CategoryDropdown.Select();
                isCategoryDropdownFocused = true;
            }
            else if (isCategoryDropdownFocused)
            {
                CreatePlayerButton.Select();
                isCategoryDropdownFocused = false;
            }   
            else
                PlayerNameInput.Select();
        }
    }

    public override void Show(bool flag = true)
    {
        if (flag)
        {
            IsDone = false;
        }

        gameObject.SetActive(flag);
        IsShown = flag;
    }

    private void OnCreatePlayer()
    {
        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var age = 0;

        var isRaceValid = race != null && string.IsNullOrEmpty(race.Id);
        var isNameValid = PlayerNameInput.text.Length > 0;
        var isTeamNameValid = TeamNameInput.text.Length > 0;
        var isAgeValid = AgeInput.text.Length > 0 && int.TryParse(AgeInput.text, out age);
        var isEmailValid = EmailInput.text.Length > 0;

        PlayerNameInput.GetComponent<Image>().color = isNameValid ? ValidBgColor : InvalidBgColor;
        TeamNameInput.GetComponent<Image>().color = isTeamNameValid ? ValidBgColor : InvalidBgColor;
        AgeInput.GetComponent<Image>().color = isAgeValid ? ValidBgColor : InvalidBgColor;
        EmailInput.GetComponent<Image>().color = isEmailValid ? ValidBgColor : InvalidBgColor;

        if (!isRaceValid || !isNameValid || !isTeamNameValid || !isAgeValid || !isEmailValid)
            return;

        try
        {
            var playerInfo = RaceTimerServices.GetInstance().RaceService.CreatePlayer(
                                race.Id,
                                PlayerNameInput.text,
                                age,
                                EmailInput.text,
                                TeamNameInput.text);

            if (playerInfo != null)
                IsDone = true;
            else
                throw new Exception("Failed to create race");
        }
        catch (Exception ex)
        {
            throw new UnityException(ex.Message);
        }

    }

}

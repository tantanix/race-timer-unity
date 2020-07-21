using Tcs.Unity;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CreatePlayerForm
{
    public string RaceId { get; set; }
    public string Name { get; set; }
    public string TeamName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

public class CreatePlayerDialog : MonoBehaviour
{
    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_InputField PlayerNameInput;
    public TMP_InputField TeamNameInput;
    public TMP_InputField AgeInput;
    public TMP_InputField EmailInput;
    public TMP_Dropdown CategoryDropdown;
    public Button CreatePlayerButton;
    public Button CloseButton;

    private bool isCategoryDropdownFocused = false;
    private List<string> _categories;

    void Start()
    {
        CreatePlayerButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => CreatePlayer());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());

        RaceTimerServices.GetInstance()?.RaceService
            .OnNewRaceCategory()
            .TakeUntilDestroy(this)
            .Subscribe(category =>
            {
                if (category != null)
                {
                    _categories.Add(category.Name);
                    UpdateCategoryList();
                }
            });

        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var raceCategories = RaceTimerServices.GetInstance().RaceService.GetAllRaceCategories(race.Id);
        _categories = raceCategories.Select(x => x.Name).ToList();

        UpdateCategoryList();
    }

    private void UpdateCategoryList()
    {
        CategoryDropdown.ClearOptions();
        CategoryDropdown.AddOptions(_categories);
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

    private void CreatePlayer()
    {
        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var age = 0;

        var isRaceValid = race != null && !string.IsNullOrEmpty(race.Id);
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
            var playerInfo = RaceTimerServices.GetInstance().RaceService.CreateRacePlayer(
                race.Id,
                PlayerNameInput.text,
                age,
                EmailInput.text,
                TeamNameInput.text);

            if (playerInfo == null)
                throw new Exception("Failed to create player");

            Close();
        }
        catch (Exception ex)
        {
            throw new UnityException("Failed to create player", ex);
        }
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

    public void Reset()
    {
        PlayerNameInput.text = "";
        TeamNameInput.text = "";
        AgeInput.text = "";
        EmailInput.text = "";
    }
}

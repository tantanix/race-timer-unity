using Tcs.Unity;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        CreatePlayerButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnCreatePlayer());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnClose());

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

    private void OnCreatePlayer()
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

        DialogService.GetInstance().Close(
            gameObject,
            true,
            new CreatePlayerForm
            {
                RaceId = race.Id,
                Name = PlayerNameInput.text,
                TeamName = TeamNameInput.text,
                Age = age,
                Email = EmailInput.text
            });
    }

    private void OnClose()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

}

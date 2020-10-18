using Tcs.Unity;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Tcs.Core.Validators;

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
    public const string SelectCategoryOption = "Select a category";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public MatInput PlayerNameInput;
    public MatInput TeamNameInput;
    public MatInput AgeInput;
    public MatInput EmailInput;
    public MatDropdown CategoryDropdown;
    public Button CreatePlayerButton;
    public Button CloseButton;

    private List<string> _categories = new List<string>();
    private string _selectedCategory;

    void Awake()
    {
        PlayerNameInput.AddValidator(Validators.RequiredInputField, "Player name is required");
        TeamNameInput.AddValidator(Validators.RequiredInputField, "Team name is required");
        AgeInput.AddValidator(Validators.RequiredAndValidNumberInputField, "Age value is not valid");
        
        EmailInput.AddValidator(Validators.RequiredInputField, "Email is required");
        EmailInput.AddValidator(Validators.EmailInputField, "Email is not valid");
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (PlayerNameInput.isFocused)
        //        TeamNameInput.Select();
        //    else if (TeamNameInput.isFocused)
        //        AgeInput.Select();
        //    else if (AgeInput.isFocused)
        //        EmailInput.Select();
        //    else if (EmailInput.isFocused)
        //    {
        //        CategoryDropdown.Select();
        //        isCategoryDropdownFocused = true;
        //    }
        //    else if (isCategoryDropdownFocused)
        //    {
        //        CreatePlayerButton.Select();
        //        isCategoryDropdownFocused = false;
        //    }   
        //    else
        //        PlayerNameInput.Select();
        //}
    }

    private void SelectCategory(int index)
    {
        if (!_categories.Any())
            return;

        _selectedCategory = _categories[index];
    }

    private void CreatePlayer()
    {
        var category = _selectedCategory != SelectCategoryOption ? _selectedCategory : null;

        var isNameValid = PlayerNameInput.Validate();
        var isTeamNameValid = TeamNameInput.Validate();
        var isAgeValid = AgeInput.Validate();
        var isEmailValid = EmailInput.Validate();

        if (!isNameValid || !isTeamNameValid || !isAgeValid || !isEmailValid)
            return;

        try
        {
            var playerInfo = RaceTimerServices.GetInstance().RaceService.CreateRacePlayer(
                PlayerNameInput.text,
                int.Parse(AgeInput.text),
                EmailInput.text,
                category,
                TeamNameInput.text);

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

    public void Initialize()
    {
        PlayerNameInput.Initialize();
        TeamNameInput.Initialize();
        AgeInput.Initialize();
        EmailInput.Initialize();
        CategoryDropdown.Initialize();

        UpdateCategoryList();

        CreatePlayerButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .TakeUntilDisable(this)
            .Subscribe(_ => CreatePlayer());

        CategoryDropdown
            .InnerDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .TakeUntilDisable(this)
            .Subscribe(index => SelectCategory(index));

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .TakeUntilDisable(this)
            .Subscribe(_ => Close());
    }

    private void UpdateCategoryList()
    {
        _categories.Clear();
        _categories.Add(SelectCategoryOption);

        var allRaceCategories = RaceTimerServices.GetInstance().RaceService.GetAllRaceCategories();

        foreach (var raceCategory in allRaceCategories)
        {
            _categories.Add(raceCategory.Category.Name);
        }

        CategoryDropdown.InnerDropdown.ClearOptions();
        CategoryDropdown.InnerDropdown.AddOptions(_categories);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.Core.Validators;
using Tcs.Unity;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public struct AddCategoryForm
{
    public string Name;
}

public class AddCategoryDialog : MonoBehaviour
{
    public const string SelectCategoryOption = "Select a category";
    public const string CreateNewCategoryOption = "Create new category";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public MatDropdown CategoryDropdown;
    public MatInput NameInput;
    public Button CreateButton;
    public Button CloseButton;

    private List<string> _categories = new List<string>();
    private string _selectedCategory;

    void Awake()
    {
        CreateButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => CreateCategory());

        CategoryDropdown
            .InnerDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectCategory(index));

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());

        CategoryDropdown.AddValidator(Validators.RequiredDropdown, "Category is required");
        NameInput.AddValidator(Validators.RequiredInputField, "Category name is required");
    }

    public void Initialize()
    {
        _selectedCategory = "";
        NameInput.text = "";
        NameInput.gameObject.SetActive(false);

        InitializeCategoryDropdown();
    }

    private void CreateCategory()
    {
        var isCategoryValid = CategoryDropdown.Validate();
        if (!isCategoryValid)
            return;

        string name;
        if (_selectedCategory == CreateNewCategoryOption)
        {
            var isNameValid = NameInput.Validate();
            if (!isNameValid)
                return;

            name = NameInput.text;
        }
        else
            name = _selectedCategory;

        try
        {
            var newCategory = RaceTimerServices.GetInstance().RaceService.AddRaceCategory(name);
            if (newCategory == null)
                throw new Exception("Failed to create category");

            Close();
        }
        catch (Exception ex)
        {
            throw new UnityException("Failed to create player", ex);
        }
    }

    private void SelectCategory(int index)
    {
        if (!_categories.Any())
            return;

        _selectedCategory = _categories[index];
        if (_selectedCategory == CreateNewCategoryOption)
        {
            NameInput.gameObject.SetActive(true);
            _selectedCategory = CreateNewCategoryOption;
        }
        else if (_selectedCategory == SelectCategoryOption)
        {
            NameInput.gameObject.SetActive(false);
            NameInput.text = "";
            _selectedCategory = SelectCategoryOption;
        }
        else
        {
            NameInput.gameObject.SetActive(false);
        }

        CategoryDropdown.Validate();
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

    private void InitializeCategoryDropdown()
    {
        var currentRace = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var allCategories = RaceTimerServices.GetInstance().RaceService.GetAllCategories();
        var raceCategories = RaceTimerServices.GetInstance().RaceService.GetAllRaceCategories().ToList();

        _categories.Clear();
        _categories.Add(SelectCategoryOption);

        foreach (var category in allCategories)
        {
            if (!raceCategories.Exists(x => x.Category.Id == category.Id))
            {
                _categories.Add(category.Name);
            }
        }

        _categories.Add(CreateNewCategoryOption);

        CategoryDropdown.InnerDropdown.ClearOptions();
        CategoryDropdown.InnerDropdown.AddOptions(_categories);
    }
}

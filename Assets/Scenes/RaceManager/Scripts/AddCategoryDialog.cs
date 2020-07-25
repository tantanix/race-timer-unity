using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.Unity;
using TMPro;
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

    public TMP_Dropdown CategoryDropdown;
    public TMP_InputField NameInput;
    public Button CreateButton;
    public Button CloseButton;

    private List<string> _categories = new List<string>();
    private string _selectedCategory;

    void Start()
    {
        CreateButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => CreateCategory());

        CategoryDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(index => SelectCategory(index));

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());

        NameInput
            .OnValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(NameInputValueChanged);
    }

    private void CreateCategory()
    {
        string name;
        if (_selectedCategory == CreateNewCategoryOption)
            name = NameInput.text;
        else
            name = _selectedCategory;

        var isNameValid = name.Length > 0;
        
        NameInput.GetComponent<Image>().color = isNameValid ? ValidBgColor : InvalidBgColor;
        
        if (!isNameValid)
            return;

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

    private void NameInputValueChanged(string value)
    {
        CreateButton.interactable = value.Length > 0;
    }

    private void SelectCategory(int index)
    {
        if (!_categories.Any())
            return;

        _selectedCategory = _categories[index];
        if (_selectedCategory == CreateNewCategoryOption)
        {
            CreateButton.interactable = false;
            NameInput.gameObject.SetActive(true);
            _selectedCategory = CreateNewCategoryOption;
        }
        else if (_selectedCategory == SelectCategoryOption)
        {
            CreateButton.interactable = false;
            NameInput.gameObject.SetActive(false);
            NameInput.text = "";
            _selectedCategory = SelectCategoryOption;
        }
        else
        {
            CreateButton.interactable = true;
            NameInput.gameObject.SetActive(false);
        }
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

    public void Initialize()
    {
        _selectedCategory = "";
        NameInput.text = "";
        NameInput.gameObject.SetActive(false);

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

        CategoryDropdown.ClearOptions();
        CategoryDropdown.AddOptions(_categories);

        CreateButton.interactable = false;
    }
}

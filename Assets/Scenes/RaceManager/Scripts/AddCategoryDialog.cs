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
    public const string CreateNewCategoryOption = "Create new category";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_Dropdown CategoryDropdown;
    public TMP_InputField NameInput;
    public Button CreateButton;
    public Button CloseButton;

    private List<string> _categories;
    private string _selectedCategory;

    void Awake()
    {
        gameObject.SetActive(false);

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
    }

    void Start()
    {
        _selectedCategory = "";
        NameInput.text = "";
        NameInput.gameObject.SetActive(false);

        var raceService = RaceTimerServices.GetInstance().RaceService;
        var race = raceService.CurrentRace;
        var allCategories = raceService.GetAllCategories();
        var raceCategories = raceService.GetAllRaceCategories(race.Id);
        var unassignedCategories = allCategories.Except(raceCategories).ToList();
        _categories = unassignedCategories.Select(x => x.Name).ToList();
        
        // If there are no categories yet, we default to create new category option.
        if (!_categories.Any())
        {
            NameInput.gameObject.SetActive(true);
            _selectedCategory = CreateNewCategoryOption;
        }
        
        _categories.Add(CreateNewCategoryOption);

        CategoryDropdown.AddOptions(_categories);
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
            var raceService = RaceTimerServices.GetInstance().RaceService;
            var race = raceService.CurrentRace;
            
            var newCategory = raceService.AddRaceCategory(race.Id, NameInput.text);
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
        if (_categories == null)
            return;

        _selectedCategory = _categories[index];
        if (_selectedCategory == CreateNewCategoryOption)
        {
            NameInput.gameObject.SetActive(true);
        } 
        else
        {
            NameInput.gameObject.SetActive(false);
        }
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }
}

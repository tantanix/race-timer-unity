using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AddCategoryDialog : MonoBehaviour
{
    public const string CreateNewCategoryOption = "Create new category";

    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_Dropdown CategoryDropdown;
    public TMP_InputField PlayerNameInput;
    public Button CreateButton;

    private List<string> _categories;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        CreateButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnCreateCategory());

        CategoryDropdown
            .OnTmpValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(OnSelectCategory);

        PlayerNameInput.gameObject.SetActive(false);

        var raceService = RaceTimerServices.GetInstance().RaceService;
        Debug.Log(raceService);
        var race = raceService.CurrentRace;
        Debug.Log(race);
        var allCategories = raceService.GetAllCategories();
        var raceCategories = raceService.GetAllRaceCategories(race.Id);
        Debug.Log(raceCategories);
        var unassignedCategories = allCategories.Except(raceCategories);
        Debug.Log(unassignedCategories);
        _categories = unassignedCategories.Select(x => x.Name).ToList();
        CategoryDropdown.AddOptions(_categories);
        
        _categories.Add(CreateNewCategoryOption);
    }

    private void OnCreateCategory()
    {
        throw new NotImplementedException();
    }

    private void OnSelectCategory(int index)
    {
        if (_categories == null)
            return;

        var category = _categories[index];
        if (category == CreateNewCategoryOption)
        {
            PlayerNameInput.gameObject.SetActive(true);
        } 
        else
        {
            PlayerNameInput.gameObject.SetActive(false);
        }
    }
}

using System;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabButton : MonoBehaviour
{
    public RaceCategoryViewModel RaceCategory { get; private set; }

    private Button _button;
    private IDisposable _buttonSub;
    private IDisposable _raceCategoryLoadedSub;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetRaceCategory(RaceCategoryViewModel raceCategory)
    {
        if (raceCategory != null)
        {
            RaceCategory = raceCategory;

            GetComponentInChildren<TMP_Text>().text = raceCategory.Category.Name;

            _buttonSub = _button
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => LoadCategory());

            _raceCategoryLoadedSub = RaceTimerServices.GetInstance().RaceService
                .OnRaceCategoryLoaded()
                .TakeUntilDestroy(this)
                .Subscribe(CheckAndUpdateIfCategorySame);
        }
        else
        {
            RaceCategory = null;
            _buttonSub.Dispose();
            _raceCategoryLoadedSub.Dispose();
        }
    }

    private void CheckAndUpdateIfCategorySame(RaceCategory raceCategory)
    {
        if (raceCategory == null || RaceCategory == null)
            return;

        if (raceCategory.Id != RaceCategory.Id)
        {
            _button.interactable = true;
            return;
        }

        _button.interactable = false;
    }

    private void LoadCategory()
    {
        RaceTimerServices.GetInstance().RaceService.LoadRaceCategory(RaceCategory.Id);
    }

}

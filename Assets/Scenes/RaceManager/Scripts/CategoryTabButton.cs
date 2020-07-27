using System;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabButton : MonoBehaviour
{
    private Button _button;
    private RaceCategoryViewModel _raceCategory;
    private IDisposable _buttonSub;
    private IDisposable _raceCategoryLoadedSub;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void CheckAndUpdateIfCategorySame(RaceCategory raceCategory)
    {
        if (raceCategory == null || _raceCategory == null)
            return;

        if (raceCategory.Id != _raceCategory.Id)
        {
            _button.interactable = true;
            return;
        }

        _button.interactable = false;
    }

    private void LoadCategory()
    {
        RaceTimerServices.GetInstance().RaceService.LoadRaceCategory(_raceCategory.Id);
    }

    public void SetRaceCategory(RaceCategoryViewModel raceCategory)
    {
        if (raceCategory != null)
        {
            _raceCategory = raceCategory;

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
            _raceCategory = null;
            _buttonSub.Dispose();
            _raceCategoryLoadedSub.Dispose();
        }
        
    }
}

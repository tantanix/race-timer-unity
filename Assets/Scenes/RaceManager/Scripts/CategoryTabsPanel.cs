using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using TMPro;
using UniRx;
using UnityEngine;

public class CategoryTabsPanel : MonoBehaviour
{
    public Transform CategoryButtonContainer;

    private List<GameObject> _buttonInstances = new List<GameObject>();

    void Awake()
    {
        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceLoaded()
            .TakeUntilDestroy(this)
            .Subscribe(race =>
            {
                if (race == null)
                    return;

                gameObject.SetActive(true);
                
                ClearList();
                LoadRaceCategories(race);
            });

        RaceTimerServices.GetInstance().RaceService
            .OnNewRaceCategory()
            .TakeUntilDestroy(this)
            .Subscribe(AddRaceCategoryButton);
    }

    private void LoadRaceCategories(RaceViewModel raceViewModel)
    {
        foreach (var category in raceViewModel.RaceCategories)
        {
            AddRaceCategoryButton(category);
        }
    }

    private void AddRaceCategoryButton(RaceCategoryViewModel raceCategory)
    {
        if (raceCategory == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("CategoryTabButton", false);
        go.GetComponentInChildren<TMP_Text>().text = raceCategory.Category.Name;
        go.transform.localScale = Vector3.one;
        go.transform.SetParent(CategoryButtonContainer, false);
        //go.transform.SetSiblingIndex(1);

        go.GetComponent<CategoryTabButton>().RaceCategory = raceCategory;

        _buttonInstances.Add(go);
    }

    private void ClearList()
    {
        foreach (var instance in _buttonInstances)
        {
            instance.transform.SetParent(null);
            ObjectPool.GetInstance().PoolObject(instance);
        }

        _buttonInstances.Clear();
    }
}

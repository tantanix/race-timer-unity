using System.Collections.Generic;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
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

    private void LoadRaceCategories(Race race)
    {
        var raceCategories = RaceTimerServices.GetInstance().RaceService.GetAllRaceCategories();
        foreach (var category in raceCategories)
        {
            AddRaceCategoryButton(category);
        }
    }

    private void AddRaceCategoryButton(RaceCategoryViewModel raceCategory)
    {
        if (raceCategory == null)
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("CategoryTabButton", false);
        go.GetComponent<CategoryTabButton>().SetRaceCategory(raceCategory);

        go.transform.localScale = Vector3.one;
        go.transform.SetParent(CategoryButtonContainer, false);

        _buttonInstances.Add(go);
    }

    private void ClearList()
    {
        foreach (var instance in _buttonInstances)
        {
            instance.transform.SetParent(null);
            instance.GetComponent<CategoryTabButton>().SetRaceCategory(null);
            ObjectPool.GetInstance().PoolObject(instance);
        }

        _buttonInstances.Clear();
    }
}

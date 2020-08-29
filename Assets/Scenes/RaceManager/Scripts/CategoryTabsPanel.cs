using System.Collections.Generic;
using System.Linq;
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

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceCategoryDeleted()
            .TakeUntilDestroy(this)
            .Subscribe(RemoveRaceCategoryButton);
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

    private void RemoveRaceCategoryButton(string raceCategoryId)
    {
        GameObject found = null;
        foreach (var button in _buttonInstances)
        {
            if (button.GetComponent<CategoryTabButton>().RaceCategory.Id == raceCategoryId)
            {
                found = button;
                ObjectPool.GetInstance().PoolObject(button);
            }
        }

        if (found != null)
            _buttonInstances.Remove(found);
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

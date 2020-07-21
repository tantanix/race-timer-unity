﻿using System;
using System.Collections.Generic;
using Tcs.RaceTimer.Models;
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

    private void LoadRaceCategories(Race race)
    {
        var categories = RaceTimerServices.GetInstance()?.RaceService.GetAllRaceCategories(race.Id);
        foreach (var category in categories)
        {
            AddRaceCategoryButton(category);
        }
    }

    private void AddRaceCategoryButton(Category category)
    {
        if (category == null)
            return;

        if (string.IsNullOrEmpty(category.Name))
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("CategoryTabButton", false);
        go.GetComponentInChildren<TMP_Text>().text = category.Name;
        go.transform.localScale = Vector3.one;
        go.transform.SetParent(CategoryButtonContainer, false);
        //go.transform.SetSiblingIndex(1);

        go.GetComponent<CategoryTabButton>().Category = category;

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
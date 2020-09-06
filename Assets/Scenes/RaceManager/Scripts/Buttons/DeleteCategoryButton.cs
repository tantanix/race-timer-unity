using System;
using Tcs.RaceTimer.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCategoryButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowConfirmationDialog());
    }

    private void ShowConfirmationDialog()
    {
        var raceService = RaceTimerServices.GetInstance().RaceService;
        var currentRaceCategory = raceService.CurrentRaceCategory;
        var category = raceService.GetCategory(currentRaceCategory.CategoryId);

        var go = ObjectPool.GetInstance().GetObjectForType("ConfirmationDialog", true);
        go.GetComponent<ConfirmationDialog>().Initialize("Confirm Delete", $"Are you sure you want to delete the category {category.Name}?");

        var dialog = DialogService.GetInstance().Show(go);

        dialog
            .AfterClosed()
            .TakeUntilDestroy(dialog)
            .Subscribe(data =>
            {
                var confirm = Convert.ToBoolean(data);
                if (confirm)
                {
                    ConfirmDeleteCategory(currentRaceCategory);
                }
            });
    }

    private void ConfirmDeleteCategory(RaceCategory currentRaceCategory)
    {
        RaceTimerServices.GetInstance().RaceService.DeleteRaceCategory(currentRaceCategory);
    }
}

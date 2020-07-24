using System;
using Tcs.RaceTimer.ViewModels;
using TMPro;
using UniRx;
using UnityEngine;

public class RaceDetailsPanel : MonoBehaviour
{
    public TMP_Text RaceNameText;
    public TMP_Text EventDateText;
    public TMP_Text LocationText;

    void Awake()
    {
        gameObject.SetActive(false);

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
                SetRaceDetails(race);
            });
    }

    public void SetRaceDetails(RaceViewModel raceModel)
    {
        RaceNameText.text = $"{raceModel.Race.Name} - {raceModel.Race.Stages} Stages".ToUpperInvariant();

        var date = new DateTime(raceModel.Race.EventDate);
        EventDateText.text = $"{date:dddd, dd MMMM yyyy h:mm tt}";

        LocationText.text = $"{raceModel.Race.Location}";
    }

}

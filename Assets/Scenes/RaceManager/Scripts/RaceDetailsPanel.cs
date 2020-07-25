using System;
using Tcs.RaceTimer.Models;
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

    public void SetRaceDetails(Race race)
    {
        RaceNameText.text = $"{race.Name} - {race.Stages} Stages".ToUpperInvariant();

        var date = new DateTime(race.EventDate);
        EventDateText.text = $"{date:dddd, dd MMMM yyyy h:mm tt}";

        LocationText.text = $"{race.Location}";
    }

}

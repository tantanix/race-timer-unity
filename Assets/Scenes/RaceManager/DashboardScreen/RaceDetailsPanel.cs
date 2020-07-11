using System;
using Tcs.RaceTimer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceDetailsPanel : MonoBehaviour
{
    public TMP_Text RaceNameText;
    public TMP_Text EventDateText;
    public TMP_Text LocationText;

    public void SetRaceDetails(Race race)
    {
        RaceNameText.text = $"{race.Name} - {race.Stages} Stages";

        var date = new DateTime(race.EventDate);
        EventDateText.text = $"{date:dddd, dd MMMM yyyy h:mm tt}";

        LocationText.text = $"{race.Location}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }
}

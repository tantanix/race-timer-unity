using System;
using Tcs.RaceTimer.Models;
using TMPro;
using UnityEngine;

public class RacePlayerEntry : MonoBehaviour
{
    public TMP_Text NameText;

    public void SetInfo(RacePlayerInfo racePlayer)
    {
        NameText.text = racePlayer.Player.Name;
    }
}

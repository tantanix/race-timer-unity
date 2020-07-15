using System;
using Tcs.RaceTimer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RacePlayerEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text NameText;
    public GameObject HoverBg;

    void Start()
    {
        HoverBg.SetActive(false);
    }

    public void SetInfo(RacePlayerInfo racePlayer)
    {
        NameText.text = racePlayer.Player.Name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverBg.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverBg.SetActive(false);
    }
}

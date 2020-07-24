using Tcs.RaceTimer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text NameText;
    public GameObject HoverBg;

    void Start()
    {
        HoverBg.SetActive(false);
    }

    public void SetInfo(Player player)
    {
        NameText.text = player.Name;
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

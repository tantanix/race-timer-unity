using System;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    public enum Panel
    {
        None,
        CreateRace
    }

    public CreateRacePanel CreateRacePanel;

    void Awake()
    {
        ShowPanel(Panel.None);
    }

    public void ShowPanel(Panel panel)
    {
        CreateRacePanel.gameObject.SetActive(false);
        
        switch (panel)
        {
            case Panel.CreateRace: CreateRacePanel.gameObject.SetActive(true); break;
            default: break;
        }
    }
}

using UnityEngine;

public class MainPanel : MonoBehaviour
{
    public CreateRacePanel CreateRacePanel;
    public RaceDashboardPanel RaceDashboardPanel;

    public void ShowAllPanels(bool flag)
    {
        CreateRacePanel.Show(flag);
        RaceDashboardPanel.Show(flag);
    }
}

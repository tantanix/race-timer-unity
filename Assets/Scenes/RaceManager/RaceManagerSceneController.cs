using System.Collections;
using UnityEngine;

public class RaceManagerSceneController : MonoBehaviour
{
    public DashboardScreen DashboardScreen;

    public enum ScreenState
    {
        Dashboard,
        CreateRace
    }

    public ScreenState? CurrenState = null;

    public void ChangeState(ScreenState state)
    {
        CurrenState = state;
        StartCoroutine($"{CurrenState}State");
    }

    IEnumerator DashboardState()
    {
        yield break;
    }

    IEnumerator CreateRaceState()
    {
        DashboardScreen.MainPanel.ShowPanel(MainPanel.Panel.CreateRace);
        yield break;
    }
}

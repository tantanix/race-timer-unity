using System.Collections;
using UnityEngine;

public class RaceManagerSceneController : MonoBehaviour
{
    public DashboardScreen DashboardScreen;

    public enum ScreenState
    {
        Initialize,
        Dashboard,
        CreateRace,
        LoadRace
    }

    public ScreenState? CurrenState = null;

    public void ChangeState(ScreenState state)
    {
        CurrenState = state;
        StartCoroutine($"{CurrenState}State");
    }

    IEnumerator InitializeState()
    {
        Debug.Log("InitializeState");
        DashboardScreen.NavPanel.Initialize();

        ChangeState(ScreenState.Dashboard);
        yield break;
    }

    IEnumerator DashboardState()
    {
        DashboardScreen.MainPanel.ShowAllPanels(false);
        yield break;
    }

    IEnumerator CreateRaceState()
    {
        DashboardScreen.MainPanel.ShowAllPanels(false);
        DashboardScreen.MainPanel.CreateRacePanel.Show();

        var createRacePanel = DashboardScreen.MainPanel.CreateRacePanel;
        while (!createRacePanel.IsDone)
        {
            yield return null;
        }

        DashboardScreen.MainPanel.CreateRacePanel.Show(false);
    }

    IEnumerator LoadRaceState()
    {
        DashboardScreen.MainPanel.ShowAllPanels(false);

        DashboardScreen.MainPanel.RaceDashboardPanel.Show(true);

        var currentRace = RaceTimerServices.GetInstance()?.RaceService.CurrentRace;
        DashboardScreen.MainPanel.RaceDashboardPanel.LoadRace(currentRace);
        yield break;
    }
}

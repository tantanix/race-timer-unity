using System.Collections;
using Tcs.RaceTimer.Models;
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
    private Race _currentRace;

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
        var race = RaceTimerServices.GetInstance()?.RaceService.CurrentRace;
        if (race == _currentRace)
            yield break;

        DashboardScreen.MainPanel.ShowAllPanels(false);

        DashboardScreen.MainPanel.RaceDashboardPanel.Show(true);
        DashboardScreen.MainPanel.RaceDashboardPanel.RaceDetailsPanel.SetRaceDetails(race);

        _currentRace = race;
        yield break;
    }
}

using System.Collections;
using UnityEngine;

public class RaceManagerSceneController : MonoBehaviour
{
    public DashboardScreen DashboardScreen;

    public enum ScreenState
    {
        Dashboard,
        CreateRace,
        LoadRace
    }

    public ScreenState? CurrenState = null;

    private RaceTimerServices _raceTimerServices;

    void Awake()
    {
        _raceTimerServices = FindObjectOfType<RaceTimerServices>();
    }

    public void ChangeState(ScreenState state)
    {
        CurrenState = state;
        StartCoroutine($"{CurrenState}State");
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

        var createdRace = createRacePanel.CreatedRace;
        var raceService = _raceTimerServices.RaceService;
        raceService.CreateRace(createdRace);

        DashboardScreen.MainPanel.CreateRacePanel.Show(false);
    }

    IEnumerator LoadRaceState()
    {
        DashboardScreen.MainPanel.ShowAllPanels(false);

        DashboardScreen.MainPanel.RaceDashboardPanel.Show(true);
        yield break;
    }
}

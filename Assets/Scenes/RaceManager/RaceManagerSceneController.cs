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
        DashboardScreen.MainPanel.CreateRacePanel.Hide();
        yield break;
    }

    IEnumerator CreateRaceState()
    {
        DashboardScreen.MainPanel.CreateRacePanel.Show();

        var createRacePanel = DashboardScreen.MainPanel.CreateRacePanel;
        while (!createRacePanel.IsDone)
        {
            yield return null;
        }

        var createdRace = createRacePanel.CreatedRace;
        var raceService = _raceTimerServices.RaceService;
        raceService.CreateRace(createdRace);

        DashboardScreen.MainPanel.CreateRacePanel.Hide();

    }
}

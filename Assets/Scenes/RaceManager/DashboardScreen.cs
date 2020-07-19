using UniRx;
using UnityEngine;

public class DashboardScreen : MonoBehaviour
{
    public RaceDashboardPanel RaceDashboardPanel;
    public RaceDetailsPanel RaceDetailsPanel;

    void Start()
    {
        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceLoaded
            .TakeUntilDestroy(this)
            .Subscribe(race =>
            {
                if (race == null)
                    return;

                Debug.Log("Race loaded: " + race);
                RaceDashboardPanel.gameObject.SetActive(true);

                RaceDetailsPanel.gameObject.SetActive(true);
                RaceDetailsPanel.SetRaceDetails(race);
            });
    }
}

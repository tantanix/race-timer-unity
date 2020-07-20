using UniRx;
using UnityEngine;

public class RaceDashboardPanel : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);

        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnRaceLoaded()
            .TakeUntilDestroy(this)
            .Subscribe(race =>
            {
                if (race == null)
                    return;

                gameObject.SetActive(true);
            });
    }
}

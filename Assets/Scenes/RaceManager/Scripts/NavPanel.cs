using TMPro;
using UnityEngine;
using Tcs.RaceTimer.Models;
using UniRx;
using UnityEngine.UI;

public class NavPanel : MonoBehaviour
{
    public Button CreateRaceButton;
    public RectTransform ButtonContainer;

    void Start()
    {
        CreateRaceButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OpenCreateRaceDialog());

        RaceTimerServices.GetInstance()?.RaceService
            .OnNewRace()
            .TakeUntilDestroy(this)
            .Subscribe(AddRaceButton);

        var races = RaceTimerServices.GetInstance()?.RaceService.GetAllRaces();
        foreach (var race in races)
        {
            AddRaceButton(race);
        }
    }

    private void AddRaceButton(Race race)
    {
        if (race == null)
            return;

        if (string.IsNullOrEmpty(race.Name))
            return;

        var go = ObjectPool.GetInstance().GetObjectForType("RaceButton", false);
        go.GetComponentInChildren<TMP_Text>().text = race.Name.Substring(0, 1).ToUpperInvariant();
        go.transform.localScale = Vector3.one;
        go.transform.SetParent(ButtonContainer, false);
        go.transform.SetSiblingIndex(1);

        go.GetComponent<RaceButton>().Race = race;
    }

    private void OpenCreateRaceDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreateRaceDialog", true);
        DialogService.GetInstance().Show(go);
    }
}

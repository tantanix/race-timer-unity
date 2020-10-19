using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerNumberButton : MonoBehaviour
{
    public TMP_InputField PlayerNoInput;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetPlayerNumber());

        PlayerNoInput.interactable = false;
        _button.interactable = false;
    }

    private void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
            return;

        RaceTimerServices.GetInstance()
            .RaceService
            .OnStageSet()
            .TakeUntilDestroy(this)
            .Subscribe(stage => OnStageSet(stage));
    }

    private void Update()
    {
        if (PlayerNoInput.text == "" && _button.interactable)
        {
            _button.interactable = false;
        }
        else if (PlayerNoInput.text != "" && !_button.interactable)
        {
            _button.interactable = true;
        }
    }

    private void OnStageSet(int stage)
    {
        var raceService = RaceTimerServices.GetInstance().RaceService;
        var raceId = raceService.CurrentRace.Id;
        var unassignedRacePlayerTimes = raceService.GetAllUnassignedRacePlayerTimes(raceId, stage);
        var hasUnassignedRacePlayerTimes = unassignedRacePlayerTimes.Any();

        PlayerNoInput.interactable = hasUnassignedRacePlayerTimes;
    }

    private void SetPlayerNumber()
    {
        var raceService = RaceTimerServices.GetInstance().RaceService;
        var raceId = raceService.CurrentRace.Id;
        var stage = raceService.CurrentStage;
        RaceTimerServices.GetInstance()
            .RaceService
            .UpdatePlayerNoOfFirstUnassignedRacePlayerTime(raceId, stage, PlayerNoInput.text);

        PlayerNoInput.text = "";
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RaceButton : MonoBehaviour
{
    public Race Race;

    private RaceManagerSceneController _controller;
    private RaceTimerServices _raceTimerServices;
    private Button _button;

    void Awake()
    {
        _controller = FindObjectOfType<RaceManagerSceneController>();
        _raceTimerServices = FindObjectOfType<RaceTimerServices>();
        _button = this.GetComponent<Button>();
    }

    void Start()
    {
        _button.onClick.AddListener(OnClickRaceButton);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnClickRaceButton()
    {
        if (Race == null)
            throw new UnityException("No race data mapped to this button");

        _raceTimerServices.LoadRace(Race);
        _controller.ChangeState(RaceManagerSceneController.ScreenState.LoadRace);
    }
}

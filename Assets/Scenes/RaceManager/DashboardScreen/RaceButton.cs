using Tcs.RaceTimer.Models;
using UnityEngine;
using UnityEngine.UI;

public class RaceButton : MonoBehaviour
{
    public Race Race;

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        GetComponent<Image>().SetNativeSize();
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

        RaceTimerServices.GetInstance().LoadRace(Race);

        var controller = FindObjectOfType<RaceManagerSceneController>();
        controller.ChangeState(RaceManagerSceneController.ScreenState.LoadRace);
    }
}

using Tcs.RaceTimer.Models;
using UniRx;
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

        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ =>
            {
                RaceTimerServices.GetInstance().RaceService.LoadRace(Race.Id);
            });
    }
}

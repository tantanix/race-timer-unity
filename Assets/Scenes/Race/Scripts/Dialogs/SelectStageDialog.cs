using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageDialog : MonoBehaviour
{
    public Button StageButton1;
    public Button StageButton2;
    public Button StageButton3;
    public Button StageButton4;
    public Button StageButton5;

    private void Awake()
    {
        StageButton1
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetStage(1));

        StageButton2
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetStage(2));

        StageButton3
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetStage(3));

        StageButton4
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetStage(4));

        StageButton5
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => SetStage(5));
    }

    public void Initialize()
    {
        var race = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        var stages = race.Stages;
        StageButton1.gameObject.SetActive(stages >= 1);
        StageButton2.gameObject.SetActive(stages >= 2);
        StageButton3.gameObject.SetActive(stages >= 3);
        StageButton4.gameObject.SetActive(stages >= 4);
        StageButton5.gameObject.SetActive(stages >= 5);
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

    private void SetStage(int stage)
    {
        RaceTimerServices.GetInstance().RaceService.SetStage(stage);
        Close();
    }
}

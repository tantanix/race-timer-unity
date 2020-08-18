using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CreateRaceButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OpenCreateRaceDialog());
    }

    private void OpenCreateRaceDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreateRaceDialog", true);
        go.GetComponent<CreateRaceDialog>().CreateNewRace();
        DialogService.GetInstance().Show(go);
    }
}

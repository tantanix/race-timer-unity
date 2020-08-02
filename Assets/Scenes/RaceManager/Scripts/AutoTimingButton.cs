using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AutoTimingButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowAutoTimingDialog());
    }

    private void ShowAutoTimingDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("AutoTimingDialog", true);
        go.GetComponent<AutoTimingDialog>().Initialize();

        DialogService.GetInstance().Show(go);
    }
}

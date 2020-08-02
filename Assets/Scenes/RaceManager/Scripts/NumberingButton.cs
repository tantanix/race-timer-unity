using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class NumberingButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowPlayerNumberingDialog());
    }

    private void ShowPlayerNumberingDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("PlayerNumberingDialog", true);
        go.GetComponent<PlayerNumberingDialog>().Initialize();

        DialogService.GetInstance().Show(go);
    }
}

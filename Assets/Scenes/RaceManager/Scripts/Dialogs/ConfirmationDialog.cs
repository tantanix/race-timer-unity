using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    public Button YesButton;
    public Button CloseButton;
    public TMP_Text TitleText;
    public TMP_Text MessageText;

    private void Awake()
    {
        YesButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Confirm());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());
    }

    public void Initialize(string title, string message)
    {
        TitleText.text = title;
        MessageText.text = message;
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true, false);
    }

    private void Confirm()
    {
        DialogService.GetInstance().Close(gameObject, true, true);
    }
}

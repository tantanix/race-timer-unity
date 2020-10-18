using Tcs.Core.Validators;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumberingDialog : MonoBehaviour
{
    public MatInput StartNumberInput;
    public Button SetButton;
    public Button CloseButton;

    private Subject<bool> _unsubscribe;

    private void Awake()
    {
        StartNumberInput.AddValidator(Validators.RequiredAndValidNumberInputField, "Start number must be a valid number");
    }

    public PlayerNumberingDialog Initialize()
    {
        _unsubscribe = new Subject<bool>();

        StartNumberInput.Initialize();

        SetButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .TakeUntil(_unsubscribe)
            .Subscribe(_ => OnSet());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .TakeUntil(_unsubscribe)
            .Subscribe(_ => Close());

        return this;
    }

    private void OnSet()
    {
        var isStartNumberValid = StartNumberInput.Validate();

        if (!isStartNumberValid)
            return;

        if (int.TryParse(StartNumberInput.text, out var startCount))
        {
            RaceTimerServices.GetInstance().RaceService.SetAutoNumbering(startCount);
            Close();
        }
        
    }

    private void Close()
    {
        _unsubscribe.OnNext(true);
        _unsubscribe.OnCompleted();

        DialogService.GetInstance().Close(gameObject, true);
    }
}

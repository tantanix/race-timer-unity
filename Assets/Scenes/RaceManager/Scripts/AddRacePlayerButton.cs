using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AddRacePlayerButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowAddRacePlayerDialog());
    }

    private void ShowAddRacePlayerDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreatePlayerDialog", true);
        go.GetComponent<CreatePlayerDialog>().Reset();

        DialogService.GetInstance().Show(go);
    }
    
}
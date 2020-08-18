using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EditRaceButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => EditRace());
    }

    private void EditRace()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreateRaceDialog", true);
        go.GetComponent<CreateRaceDialog>().EditCurrentRace();
        DialogService.GetInstance().Show(go);
    }
}

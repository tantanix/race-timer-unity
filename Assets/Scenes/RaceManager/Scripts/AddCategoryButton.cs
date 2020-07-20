using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AddCategoryButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => ShowAddCategoryDialog());
    }

    private void ShowAddCategoryDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("AddCategoryDialog", true);
        DialogService.GetInstance().Show(go);
    }
}

using System;
using UniRx;
using UnityEngine;

public class CategoryTabsPanel : MonoBehaviour
{
    public Transform CategoryButtonContainer;

    public void OnCreateCategory()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("AddCategoryDialog", true);
        var dialogRef = DialogService.GetInstance().Show(go);

        dialogRef
            .AfterClosed()
            .TakeUntilDestroy(this)
            .Subscribe(data =>
            {
                if (data != null)
                    OnCategoryCreated(data);
            });
    }

    private void OnCategoryCreated(object data)
    {
        throw new NotImplementedException();
    }
}

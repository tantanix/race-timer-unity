using UnityEngine;

public class RaceDetailsButtonPanel : MonoBehaviour
{
    public void OnCreatePlayer()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("CreatePlayerDialog", true);
        DialogService.GetInstance().Show(go);
    }
}

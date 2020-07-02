using UnityEngine;

public class PlayerLoggingPanel : MonoBehaviour
{
    public Transform LogsContainerTransform;
    
    private Clock _clock;

    void Awake()
    {
        _clock = FindObjectOfType<Clock>();
    }

    public void OnLogRider()
    {
        var entry = CreateEntry();

        entry.transform.SetParent(LogsContainerTransform, false);
        entry.transform.SetSiblingIndex(0);
    }

    private GameObject CreateEntry()
    {
        var go = ObjectPool.Instance.GetObjectForType("PlayerLogEntry", false);
        go.transform.localScale = new Vector3(1, 1, 1);

        var component = go.GetComponent<PlayerLogEntry>();
        component.SetLogTime(_clock.CurrentTime);
        return go;
    }
}

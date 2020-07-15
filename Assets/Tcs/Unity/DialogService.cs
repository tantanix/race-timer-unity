using UnityEngine;

public class DialogRef
{
    GameObject source;
}

public class DialogService : MonoBehaviour
{
    public GameObject Overlay;

    private static DialogService _instance;

    public static DialogService GetInstance()
    {
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        Overlay.SetActive(false);
    }

    public void Show(GameObject go)
    {
        Overlay.SetActive(true);

        go.transform.localScale = Vector3.one;
        go.transform.SetParent(transform, false);
    }
}

using System.Collections.Generic;
using Tcs.Unity;
using UnityEngine;

public class DialogService : MonoBehaviour
{
    private static DialogService _instance;

    public GameObject Overlay;

    private readonly List<Dialog> _dialogInstances = new List<Dialog>();
    
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
    }

    void Start()
    {
        Overlay.SetActive(false);
    }

    public Dialog Show(GameObject go)
    {
        if (go == null)
            return null;

        Overlay.SetActive(true);

        go.transform.localScale = Vector3.one;
        go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        go.transform.SetParent(transform, false);

        var dialogRef = go.AddComponent<Dialog>();
        _dialogInstances.Add(dialogRef);

        return dialogRef;
    }

    public void Close(GameObject go, bool pool = false, object data = null)
    {
        var dialogRef = go.GetComponent<Dialog>();
        
        if (dialogRef == null)
            return;

        _dialogInstances.Remove(dialogRef);

        dialogRef.Close(data);

        if (pool)
            ObjectPool.GetInstance().PoolObject(go);
        else
            Destroy(go);
        
        

        Overlay.SetActive(false);
    }

    public void CloseAll()
    {
        foreach (var dialogRef in _dialogInstances)
        {
            dialogRef.Close();
            Destroy(dialogRef.gameObject);
        }

        _dialogInstances.Clear();

        Overlay.SetActive(false);
    }
}

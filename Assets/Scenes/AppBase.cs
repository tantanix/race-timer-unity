using UnityEngine;

public abstract class AppBase : MonoBehaviour
{
    public bool IsDone { get; set; } = false;
    public bool HasSelected { get; set; } = false;
    public bool IsShown { get; set; } = false;

    public abstract void Show(bool flag = true);
}

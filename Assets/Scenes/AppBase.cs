using UnityEngine;

public abstract class AppBase : MonoBehaviour
{
    public bool IsDone = false;
    public bool HasSelected = false;
    public bool IsShown = false;

    public abstract void Show(bool flag = true);
}

using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    public void Initialize(string text)
    {
        transform.Find("InputField").GetComponent<InputField>().text = text;
    }
}

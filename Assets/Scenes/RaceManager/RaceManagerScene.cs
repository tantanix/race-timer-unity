using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManagerScene : MonoBehaviour
{
    void Awake()
    {
        if (RaceTimerServices.GetInstance() == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }
    }
}

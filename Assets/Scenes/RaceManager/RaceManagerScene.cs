using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManagerScene : MonoBehaviour
{
    void Awake()
    {
        if (RaceTimerServices.GetInstance() == null)
            SceneManager.LoadScene("MainScene");

        var controller = FindObjectOfType<RaceManagerSceneController>();
        controller.ChangeState(RaceManagerSceneController.ScreenState.Dashboard);
    }
}

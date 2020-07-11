using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManagerScene : MonoBehaviour
{
    void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }
            

        var controller = FindObjectOfType<RaceManagerSceneController>();
        controller.ChangeState(RaceManagerSceneController.ScreenState.Initialize);
    }
}

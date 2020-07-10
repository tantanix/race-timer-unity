using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
            SceneManager.LoadScene("MainScene");

        var controller = FindObjectOfType<RaceSceneController>();
        controller.ChangeState(RaceSceneController.ScreenState.Race);
    }
}

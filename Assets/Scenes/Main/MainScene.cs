using UnityEngine;

public class MainScene : MonoBehaviour
{
    void Start()
    {
        var controller = FindObjectOfType<MainSceneController>();
        controller.ChangeState(MainSceneController.ScreenState.Main);
    }
}

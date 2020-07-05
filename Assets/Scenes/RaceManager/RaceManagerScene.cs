using UnityEngine;

public class RaceManagerScene : MonoBehaviour
{
    void Awake()
    {
        var controller = FindObjectOfType<RaceManagerSceneController>();
        controller.ChangeState(RaceManagerSceneController.ScreenState.Dashboard);
    }
}

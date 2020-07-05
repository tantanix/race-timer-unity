using UnityEngine;

public class NavPanel : MonoBehaviour
{
    public RaceManagerSceneController Controller;

    public void OnCreateRace()
    {
        Controller.ChangeState(RaceManagerSceneController.ScreenState.CreateRace);
    }
}

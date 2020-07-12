using UnityEngine;

public class RaceDetailsButtonPanel : MonoBehaviour
{
    private RaceManagerSceneController _controller;

    void Awake()
    {
        _controller = FindObjectOfType<RaceManagerSceneController>();
    }

    public void OnCreatePlayer()
    {
        _controller.ChangeState(RaceManagerSceneController.ScreenState.CreatePlayer);
    }
}

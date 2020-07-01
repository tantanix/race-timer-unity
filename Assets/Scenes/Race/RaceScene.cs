using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var controller = FindObjectOfType<RaceSceneController>();
        controller.ChangeState(RaceSceneController.ScreenState.Race);
    }
}

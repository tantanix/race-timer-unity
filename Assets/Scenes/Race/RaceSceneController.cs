using System.Collections;
using UnityEngine;

public class RaceSceneController : MonoBehaviour
{
    public RaceScreen RaceScreen;
    
    public enum ScreenState
    {
        Race
    }

    public ScreenState? CurrentState = null;

    public void ChangeState(ScreenState state)
    {
        CurrentState = state;
        StartCoroutine($"{CurrentState}State");
    }

    IEnumerator RaceState()
    {
        yield break;
    }
}

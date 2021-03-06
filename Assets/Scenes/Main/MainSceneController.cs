﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    public MainScreen MainScreen;

    //public MainScreen
    public enum ScreenState
    {
        Main
    }

    public ScreenState? CurrentState = null;

    public void ChangeState(ScreenState state)
    {
        CurrentState = state;
        StartCoroutine($"{CurrentState}State");
    }

    IEnumerator MainState()
    {
        MainScreen.Initialize("Hello");
        yield break;
    }
}

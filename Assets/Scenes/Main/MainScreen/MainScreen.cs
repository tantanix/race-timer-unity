using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreen : MonoBehaviour
{
    public MainSceneController Controller;

    public void CreateRace()
    {
        SceneManager.LoadScene("RaceManagerScene");
    }
}

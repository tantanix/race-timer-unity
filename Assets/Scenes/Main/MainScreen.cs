using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    public Button OpenRaceManagerButton;

    void Start()
    {
        OpenRaceManagerButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ =>
            {
                SceneManager.LoadScene("RaceManagerScene");
            });
    }
}

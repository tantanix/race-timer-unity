using SFB;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ImportRaceButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnClick());
    }

    private void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
    }
}

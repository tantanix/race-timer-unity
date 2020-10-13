using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (RaceTimerServices.GetInstance() == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }
        
        OpenSelectStageDialog();
    }

    private void OpenSelectStageDialog()
    {
        var go = ObjectPool.GetInstance().GetObjectForType("SelectStageDialog", true);
        go.GetComponent<SelectStageDialog>().Initialize();
        DialogService.GetInstance().Show(go);
    }
}

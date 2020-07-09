using Assets.Scenes;

public class RaceDashboardPanel : AppBase
{
    public override void Show(bool flag = true)
    {
        if (flag)
        {
            IsDone = false;
        }

        gameObject.SetActive(flag);
        IsShown = flag;
    }
}

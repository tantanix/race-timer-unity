public class RaceDashboardPanel : AppBase
{
    public RaceDetailsPanel RaceDetailsPanel;

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

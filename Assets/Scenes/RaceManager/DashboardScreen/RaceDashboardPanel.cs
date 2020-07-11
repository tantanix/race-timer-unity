using Tcs.RaceTimer.Models;

public class RaceDashboardPanel : AppBase
{
    private Race _currentRace;

    public void LoadRace(Race race)
    {
        if (_currentRace == race)
            return;

        _currentRace = race;
    }

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

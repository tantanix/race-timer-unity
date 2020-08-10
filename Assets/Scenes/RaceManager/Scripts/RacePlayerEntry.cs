using System.Linq;
using Tcs.RaceTimer.Models;
using Tcs.RaceTimer.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacePlayerEntry : MonoBehaviour
{
    public Toggle Toggle;
    public TMP_Text NumberText;
    public TMP_Text NameText;
    public TMP_Text TeamText;
    public TMP_Text S1Text;
    public TMP_Text S1RaceTimeText;
    public TMP_Text S2Text;
    public TMP_Text S2RaceTimeText;
    public TMP_Text S3Text;
    public TMP_Text S3RaceTimeText;
    public TMP_Text S4Text;
    public TMP_Text S4RaceTimeText;
    public TMP_Text S5Text;
    public TMP_Text S5RaceTimeText;
    public Button EditButton;

    private void Awake()
    {
        
    }

    public void SetInfo(RacePlayerViewModel racePlayer)
    {
        if (racePlayer != null)
        {
            NameText.text = racePlayer.Player.Name;
            TeamText.text = racePlayer.Team.Name;

            S1Text.gameObject.SetActive(racePlayer.Race.Stages >= 1);
            S1RaceTimeText.gameObject.SetActive(racePlayer.Race.Stages >= 1);

            S2Text.gameObject.SetActive(racePlayer.Race.Stages >= 2);
            S2RaceTimeText.gameObject.SetActive(racePlayer.Race.Stages >= 2);

            S3Text.gameObject.SetActive(racePlayer.Race.Stages >= 3);
            S3RaceTimeText.gameObject.SetActive(racePlayer.Race.Stages >= 3);

            S4Text.gameObject.SetActive(racePlayer.Race.Stages >= 4);
            S4RaceTimeText.gameObject.SetActive(racePlayer.Race.Stages >= 4);

            S5Text.gameObject.SetActive(racePlayer.Race.Stages >= 5);
            S5RaceTimeText.gameObject.SetActive(racePlayer.Race.Stages >= 5);

            S1RaceTimeText.text = FormatRacePlayerTime(racePlayer.PlayerTimes?.FirstOrDefault(x => x.Stage == 1));
            S2RaceTimeText.text = FormatRacePlayerTime(racePlayer.PlayerTimes?.FirstOrDefault(x => x.Stage == 2));
            S3RaceTimeText.text = FormatRacePlayerTime(racePlayer.PlayerTimes?.FirstOrDefault(x => x.Stage == 3));
            S4RaceTimeText.text = FormatRacePlayerTime(racePlayer.PlayerTimes?.FirstOrDefault(x => x.Stage == 4));
            S5RaceTimeText.text = FormatRacePlayerTime(racePlayer.PlayerTimes?.FirstOrDefault(x => x.Stage == 5));
        }
        else
        {

        }
    }

    private string FormatRacePlayerTime(RacePlayerTime racePlayerTime)
    {
        if (racePlayerTime == null)
            return "00:00:00";

        var timeOfDay = racePlayerTime.Time.Hours >= 12 ? "PM" : "AM";
        var hours = racePlayerTime.Time.Hours >= 12 ? racePlayerTime.Time.Hours - 12 : racePlayerTime.Time.Hours;

        return string.Format($"{hours:D2}:{racePlayerTime.Time.Minutes:D2}:{racePlayerTime.Time.Seconds:D2} {timeOfDay}");
    }
}

using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CreateRaceDialog : MonoBehaviour
{
    public Color32 ValidBgColor;
    public Color32 RequiredBgColor;
    public TMP_Text DialogTitleText;
    public MatInput RaceNameInput;
    public MatInput NumberOfStagesInput;
    public MatInput EventDateInput;
    public MatInput LocationInput;
    public Button CreateRaceButton;
    public Button CloseButton;

    void Start()
    {
        CreateRaceButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => CreateRace());

        CloseButton
            .OnClickAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => Close());
    }

    public CreateRaceDialog Initialize()
    {
        RaceNameInput.Initialize();
        NumberOfStagesInput.Initialize();
        EventDateInput.Initialize();
        LocationInput.Initialize();

        DialogTitleText.text = "Create Race";
        CreateRaceButton.GetComponentInChildren<TMP_Text>().text = "Create";

        return this;
    }

    public CreateRaceDialog EditCurrentRace()
    {
        var currentRace = RaceTimerServices.GetInstance().RaceService.CurrentRace;
        RaceNameInput.text = currentRace.Name;
        NumberOfStagesInput.text = currentRace.Stages.ToString();

        IFormatProvider culture = new CultureInfo("en-US", true);
        EventDateInput.text = new DateTime(currentRace.EventDate).ToString("yyyy-MM-dd", culture);
        LocationInput.text = currentRace.Location;

        DialogTitleText.text = "Edit Race";
        CreateRaceButton.GetComponentInChildren<TMP_Text>().text = "Edit";

        return this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (RaceNameInput.InnerInput.isFocused)
                NumberOfStagesInput.InnerInput.Select();
            else if (NumberOfStagesInput.InnerInput.isFocused)
                EventDateInput.InnerInput.Select();
            else if (EventDateInput.InnerInput.isFocused)
                LocationInput.InnerInput.Select();
            else if (LocationInput.InnerInput.isFocused)
                CreateRaceButton.Select();
            else
                RaceNameInput.InnerInput.Select();
        }
    }

    public void CreateRace()
    {
        var numberOfStages = 0;
        var eventDate = DateTime.Now;
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        var styles = DateTimeStyles.None;

        var isRaceNameValid = RaceNameInput.text.Length > 0;
        var isNumberOfStagesValid = NumberOfStagesInput.text.Length > 0 && int.TryParse(NumberOfStagesInput.text, out numberOfStages);
        var isEventDateValid = EventDateInput.text.Length > 0 && DateTime.TryParse(EventDateInput.text, culture, styles, out eventDate);

        if (!isRaceNameValid) RaceNameInput.Validate();
        if (!isNumberOfStagesValid) NumberOfStagesInput.Validate();
        if (!isEventDateValid) EventDateInput.Validate();

        if (!isRaceNameValid || !isNumberOfStagesValid || !isEventDateValid)
            return;

        try
        {
            var race = RaceTimerServices.GetInstance().RaceService.CreateRace(RaceNameInput.text, eventDate.Ticks, numberOfStages, LocationInput.text);
            if (race != null)
                DialogService.GetInstance().Close(gameObject, true);
            else
                throw new Exception("Failed to create race");
        } 
        catch (Exception ex)
        {
            throw new UnityException(ex.Message);
        }
    }

    private void Close()
    {
        DialogService.GetInstance().Close(gameObject, true);
    }

}

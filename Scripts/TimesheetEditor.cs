using System;
using System.IO;
using System.Text.Json;
using Godot;

public partial class TimesheetEditor : Control
{
    private readonly string timesheetDir = Path.Join(Manager.SaveDir,"timesheet_data");
    private DayOfWeek day;
    private DayOfWeek Day
    {
        get => day;
        set
        {
            day = (DayOfWeek)(((byte)value - 1 + 6) % 6 + 1);
            dayDisplay.Text = day.ToString();
        }
    }
    private Label dayDisplay;
    private LineEdit fromDate;
    private LineEdit toDate;
    private Button buttonL;
    private Button buttonR;
    private LineEdit hours;
    private LineEdit location;
    private LineEdit manager;
    private LineEdit km;
    private TimesheetEntry[] entries;
    private DateOnly currentMondayDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-((byte)DateTime.Now.DayOfWeek == 0 ? 6 : (byte)DateTime.Now.DayOfWeek - 1)));
    private string timesheetPath = Manager.AlternativeTimesheet;


    public override void _Ready()
    {
        Manager.Singleton.BeforeSceneBack += SaveTimesheet;

        dayDisplay = GetNode<Label>("%WeekDay");
        fromDate = GetNode<LineEdit>("%FromDate");
        toDate = GetNode<LineEdit>("%ToDate");
        buttonL = GetNode<Button>("%ButtonL");
        buttonR = GetNode<Button>("%ButtonR");
        hours = GetNode<LineEdit>("%Hours");
        location = GetNode<LineEdit>("%Location");
        manager = GetNode<LineEdit>("%Manager");
        km = GetNode<LineEdit>("%Km");

        Day = DateTime.Now.DayOfWeek;
        LoadTimesheet();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsKeyPressed(Key.Space))
        {
            PdfGenerator.GeneratePdf(new Timesheet
            {
                Entries = entries,
                FomDate = DateOnly.Parse(fromDate.Text)
            });
        }
    }

    public override void _ExitTree() => Manager.Singleton.BeforeSceneBack -= SaveTimesheet;

    private void LoadTimesheet()
    {
        var jsonString = Godot.FileAccess.GetFileAsString(timesheetPath ?? Path.Join(timesheetDir, $"{currentMondayDate:yyyy_MM_dd}.json"));
        if (Godot.FileAccess.GetOpenError() != Error.Ok)
        {
            CreateTimesheet();
            return;
        }

        var data = JsonSerializer.Deserialize<Timesheet>(jsonString);

        fromDate.Text = $"{data.FomDate:d}";
        toDate.Text = $"{data.FomDate.AddDays(5):d}";
        entries = data.Entries;

        LoadInputs();
    }

    private void CreateTimesheet()
    {
        timesheetPath = Path.Join(timesheetDir, $"{currentMondayDate:yyyy_MM_dd}.json");
        
        fromDate.Text = $"{currentMondayDate:d}";
        toDate.Text = $"{currentMondayDate.AddDays(5):d}";
        entries = new TimesheetEntry[6];
        Array.Fill(entries, new TimesheetEntry("", "", "", ""));
    }

    private void SaveTimesheet()
    {
        UpdateDayData();

        var data = new Timesheet
        {
            Entries = entries,
            FomDate = DateOnly.Parse(fromDate.Text)
        };

        PdfGenerator.GeneratePdf(data);

        var jsonString = JsonSerializer.Serialize(data);
        var file = Godot.FileAccess.Open(Path.Join(timesheetDir, $"{data.FomDate:yyyy_MM_dd}.json"), Godot.FileAccess.ModeFlags.Write);
        if (Godot.FileAccess.GetOpenError() != Error.Ok) return;
        file.StoreString(jsonString);
        file.Close();
    }

    private void UpdateDayData() => entries[(byte)Day - 1] = entries[(byte)Day - 1] with { Hours = hours.Text, Location = location.Text, Manager = manager.Text, Km = km.Text };


    private void LoadInputs()
    {
        if (entries[(byte)Day - 1] == null) hours.Text = location.Text = manager.Text = "";
        else
        {
            hours.Text = entries[(byte)Day - 1].Hours ?? "";
            location.Text = entries[(byte)Day - 1].Location ?? "";
            manager.Text = entries[(byte)Day - 1].Manager ?? "";
            km.Text = entries[(byte)Day - 1].Km ?? "";
        }
    }

    private void OnGoBack() => Manager.Singleton.SceneBack();

    private void OnNextDay()
    {
        UpdateDayData();
        Day++;
        LoadInputs();
    }

    private void OnPreviousDay()
    {
        UpdateDayData();
        Day--;
        LoadInputs();
    }
}

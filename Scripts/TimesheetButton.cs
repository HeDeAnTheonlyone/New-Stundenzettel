using System;
using Godot;

public partial class TimesheetButton : Control
{
    private Button edit;
    private Button send;
    [Export] private string filePath;

    public override void _Ready()
    {
        edit = GetNode<Button>("%Edit");
        send = GetNode<Button>("%Send");
        edit.Text = GetNameFromFilePath(filePath);
    }

    public void SetFilePath(string path) => filePath = path;

    private string GetNameFromFilePath(string path)
    {
        var fromDate = DateOnly.ParseExact(path.Substring(path.Length - "yyyy_MM_dd.json".Length, "yyyy_MM_dd".Length), "yyyy_MM_dd", null);
        return $"{fromDate:d} -  {fromDate.AddDays(5):d}";
    }


    private void OnEditPressed()
    {
        Manager.AlternativeTimesheet = filePath;
        Manager.Singleton.SwitchScene("TimesheetEditor");
    }
}

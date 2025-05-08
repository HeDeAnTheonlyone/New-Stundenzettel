using System.IO;
using Godot;

public partial class TimesheetButton : Control
{
    private Button edit;
    [Export] private string filePath;

    public override void _Ready()
    {
        edit = GetNode<Button>("%Edit");
        edit.Text = filePath.ExtractDateRangeFromPath(false);
    }

    public void SetFilePath(string path) => filePath = path;

    private void OnEditPressed()
    {
        Manager.AlternativeTimesheet = filePath;
        Manager.Singleton.SwitchScene("TimesheetEditor");
    }

    private async void OnSendPressed()
    {
        if (OS.GetName() == "Android")
        {
            OS.ShellOpen(Path.Join("file://", Manager.SaveDir, "stundenzettel", filePath.ConvertToTimesheetPdfPath()));
        }
        else await EmailSender.SendEmailWithAttachement($"Arbeitsbericht {Settings.UserSettings.User} [{filePath.ExtractDateRangeFromPath(true)}]");
    }
}

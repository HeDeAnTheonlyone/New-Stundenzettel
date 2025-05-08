using System.IO;
using System.Linq;
using Godot;

public partial class TimesheetList : Control
{
    private VBoxContainer list;
    private PackedScene timesheetButton = GD.Load<PackedScene>("res://Objects/TimesheetButton.tscn");

    public override void _Ready()
    {
        list = GetNode<VBoxContainer>("%List");

        var fileNames = GetSaveFileNames().OrderDescending().ToArray();
        if (fileNames != null) PopulateList(fileNames);
    }

    private void PopulateList(string[] filePaths)
    {
        foreach (var path in filePaths)
        {
            var button = timesheetButton.Instantiate<TimesheetButton>();
            button.SetFilePath(path);
            list.AddChild(button);
        }
    }

    private string[] GetSaveFileNames()
    {
        var path = Path.Join(Manager.SaveDir, "timesheet_data");
        return Directory.Exists(path)
            ? Directory.GetFiles(path)
            : null;
    }

    private void OnGoBack() => Manager.Singleton.SceneBack();
}

using Godot;

public partial class Menu : Control
{
    private void OnNewPressed() => Manager.Singleton.SwitchScene("TimesheetEditor");

    private void OnLoadPressed() => Manager.Singleton.SwitchScene("TimesheetList");

    private void OnSettingsPressed() => Manager.Singleton.SwitchScene("SettingsMenu");

    private void OnExitPressed() => GetTree().Quit();
}

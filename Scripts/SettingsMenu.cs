using Godot;

public partial class SettingsMenu : Control
{
    private LineEdit user;

    public override void _Ready()
    {
        Manager.Singleton.BeforeSceneBack += UpdateUserSettings;

        user = GetNode<LineEdit>("%User");

        LoadInputs();
    }

    public override void _ExitTree() => Manager.Singleton.BeforeSceneBack -= UpdateUserSettings;

    private void UpdateUserSettings() => Settings.Singleton.UpdateUserSettings(new UserSettings{User = user.Text});

    private void LoadInputs()
    {
        user.Text = Settings.UserSettings.User;
    }

    private void OnGoBack() => Manager.Singleton.SceneBack();
}

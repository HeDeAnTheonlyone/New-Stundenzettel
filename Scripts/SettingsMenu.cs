using Godot;

public partial class SettingsMenu : Control
{
    private LineEdit user;
    private LineEdit mailAddress;
    private LineEdit appPassword;
    private LineEdit targetAddress;

    public override void _Ready()
    {
        Manager.Singleton.BeforeSceneBack += UpdateUserSettings;

        user = GetNode<LineEdit>("%User");
        mailAddress = GetNode<LineEdit>("%MailAddress");
        appPassword = GetNode<LineEdit>("%AppPassword");
        targetAddress = GetNode<LineEdit>("%TargetAddress");

        LoadInputs();
    }

    public override void _ExitTree() => Manager.Singleton.BeforeSceneBack -= UpdateUserSettings;

    private void UpdateUserSettings() => Settings.Singleton.UpdateUserSettings(Settings.UserSettings with
    {
        User = user.Text,
        MailAddress = mailAddress.Text,
        AppPassword = appPassword.Text != "*****" ? appPassword.Text : Settings.UserSettings.AppPassword,
        TargetAddress = targetAddress.Text
    });

    private void LoadInputs()
    {
        user.Text = Settings.UserSettings.User ?? "";
        mailAddress.Text = Settings.UserSettings.MailAddress ?? "";
        appPassword.Text = "*****";
        targetAddress.Text = Settings.UserSettings.TargetAddress ?? "";
    }

    private void OnGoBack() => Manager.Singleton.SceneBack();
}

using System.Text.Json;
using Godot;

public partial class Settings: Node
{
    public static UserSettings UserSettings { get; private set; }
    private static Settings singleton;
    public static Settings Singleton
    {
        get => singleton;
        private set
        {
            if (singleton != null) return;
            singleton = value;
        }
    }

    public override void _Ready()
    {
        Singleton = this;
        LoadSettings();
    }

    public void UpdateUserSettings(UserSettings data)
    {
        UserSettings = data;
        SaveSettings();
    }


    private void LoadSettings()
    {
        var jsonText = FileAccess.GetFileAsString("user://.settings");
        if (FileAccess.GetOpenError() != Error.Ok)
        {
            CreateSettings();
            return;
        }

        UserSettings = JsonSerializer.Deserialize<UserSettings>(jsonText);
    }

    private void CreateSettings()
    {
        UserSettings = new UserSettings{User = ""};
    }

    private void SaveSettings()
    {
        var jsonText = JsonSerializer.Serialize(UserSettings);

        using var file = FileAccess.Open("user://.settings", FileAccess.ModeFlags.Write);
        if (FileAccess.GetOpenError() != Error.Ok) return;
        file.StoreString(jsonText);
    }
}

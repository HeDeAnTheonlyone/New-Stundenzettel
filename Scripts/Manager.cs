using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using PdfSharp.Fonts;

public partial class Manager : Node
{
    public static readonly string SaveDir = Path.Join(OS.GetSystemDir(OS.SystemDir.Documents), "wug");
    private static Manager singleton;
    public static Manager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton != null) return;
            singleton = value;
        }
    }
    private Stack<string> history = new();
    public static string AlternativeTimesheet { get; set; }

    public override void _Ready()
    {
        OS.RequestPermissions();
        Singleton = this;
        GetWindow().GoBackRequested += SceneBack;
        CreateDirectories();
        DownloadAssets();
        GlobalFontSettings.FontResolver = new CustomFontResolver();
        
        // GmailClient client = new();
        // _ = client.AuthenticateAsync();
    }

    public override void _ExitTree() => GetWindow().GoBackRequested -= SceneBack;

    private async void DownloadAssets()
    {
        var templateLink = "https://github.com/HeDeAnTheonlyone/stundenzettel-assets/raw/refs/heads/main/Template.jpg";
        var fontLink = "https://github.com/HeDeAnTheonlyone/stundenzettel-assets/raw/refs/heads/main/Arial.ttf";

        using System.Net.Http.HttpClient client = new();

        if (!Godot.FileAccess.FileExists("user://Template.jpg"))
        {
            var imgBytes = await client.GetByteArrayAsync(templateLink);
            using var newFile = Godot.FileAccess.Open("user://Template.jpg", Godot.FileAccess.ModeFlags.Write);
            newFile.StoreBuffer(imgBytes);
        }

        if (!Godot.FileAccess.FileExists("user://Arial.ttf"))
        {
            var fntBytes = await client.GetByteArrayAsync(fontLink);
            using var newFile = Godot.FileAccess.Open("user://Arial.ttf", Godot.FileAccess.ModeFlags.Write);
            newFile.StoreBuffer(fntBytes);
        }
    }

    private void CreateDirectories()
    {
        Directory.CreateDirectory(Path.Join(SaveDir, "timesheet_data"));
        Directory.CreateDirectory(Path.Join(SaveDir, "stundenzettel"));
    }

    public event Action BeforeSceneBack;
    
    public void SceneBack()
    {
        if (BeforeSceneBack != null) BeforeSceneBack();
        AlternativeTimesheet = null;
        if (history.Count == 0) GetTree().Quit();
        else SwitchScene(history.Pop(), false);
    }

    public void SwitchScene(string nextScene, bool addToHistory = true)
    {
        if (addToHistory)
        {
            var curScene = GetTree().CurrentScene.Name;
            if (history.Count >= 1 && curScene == history.Peek()) history.Pop();
            else history.Push(curScene);
        }

        GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, $"res://Scenes/{nextScene}.tscn");
    }
}
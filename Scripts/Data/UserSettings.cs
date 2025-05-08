using System.Text.Json.Serialization;

public record UserSettings()
{
    [JsonInclude] public string User { get; init; }
    [JsonInclude] public string MailAddress { get; init; }
    [JsonInclude] public string AppPassword { get; init; }
    [JsonInclude] public string TargetAddress { get; init; }
}

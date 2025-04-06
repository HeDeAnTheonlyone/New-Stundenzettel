
using System.Text.Json.Serialization;

public record UserSettings()
{
    [JsonInclude] public string User { get; init; }
}

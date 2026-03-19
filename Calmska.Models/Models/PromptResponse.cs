using System.Text.Json.Serialization;

namespace Calmska.Models.Models;

public class PromptResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
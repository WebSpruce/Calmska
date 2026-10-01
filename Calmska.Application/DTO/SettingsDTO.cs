using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class SettingsDTO
    {
        [Key]
        [JsonPropertyName("settingsId")]
        public Guid? SettingsId { get; set; }
        [JsonPropertyName("color")]
        public string? Color { get; set; } = string.Empty;
        [JsonPropertyName("pomodoroTimer")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public float? PomodoroTimer { get; set; }
        [JsonPropertyName("pomodoroBreak")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public float? PomodoroBreak { get; set; }
        [JsonPropertyName("userId")]
        public Guid? UserId { get; set; }
    }
}

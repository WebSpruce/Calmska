using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.DTO
{
    public class SettingsDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("settingsid")]
        public Guid? SettingsId { get; set; }
        [JsonPropertyName("color")]
        public string? Color { get; set; } = string.Empty;

        private float? _pomodoroTimer;
        private float? _pomodoroBreak;
        [JsonIgnore]
        public float? PomodoroTimerFloat
        {
            get => _pomodoroTimer;
            set => _pomodoroTimer = value;
        }
        [JsonIgnore]
        public float? PomodoroBreakFloat
        {
            get => _pomodoroBreak;
            set => _pomodoroBreak = value;
        }
        [JsonPropertyName("pomodorotimer")]
        public string? PomodoroTimer
        {
            get => _pomodoroTimer?.ToString();
            set => _pomodoroTimer = float.TryParse(value, out var result) ? result : null;
        }
        [JsonPropertyName("pomodorobreak")]
        public string? PomodoroBreak
        {
            get => _pomodoroBreak?.ToString();
            set => _pomodoroBreak = float.TryParse(value, out var result) ? result : null;
        }
        [JsonPropertyName("userid")]
        public Guid? UserId { get; set; }
    }
}

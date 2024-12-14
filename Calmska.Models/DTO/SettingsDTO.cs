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
        [JsonPropertyName("pomodorotimer")]
        public float? PomodoroTimer { get; set; } = 0.0f;
        [JsonPropertyName("pomodorobreak")]
        public float? PomodoroBreak { get; set; } = 0.0f;
        [JsonPropertyName("userid")]
        public Guid? UserId { get; set; }
    }
}

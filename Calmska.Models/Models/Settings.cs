using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class Settings
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("settingsid")]
        public Guid SettingsId { get; set; }
        [JsonPropertyName("color")]
        public required string Color { get; set; } = string.Empty;
        [JsonPropertyName("pomodorotimer")]
        public required string PomodoroTimer { get; set; } = string.Empty;
        [JsonPropertyName("pomodorobreak")]
        public required string PomodoroBreak { get; set; } = string.Empty;
        [JsonPropertyName("userid")]
        public required Guid UserId { get; set; }
        public Settings()
        {
            SettingsId = Guid.NewGuid();
        }
    }
}

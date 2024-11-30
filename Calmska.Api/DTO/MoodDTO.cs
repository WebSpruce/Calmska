using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Api.DTO
{
    public class MoodDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("moodid")]
        public Guid? MoodId { get; set; }
        [JsonPropertyName("moodname")]
        public string? MoodName { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string? Type { get; set; } = string.Empty;
    }
}

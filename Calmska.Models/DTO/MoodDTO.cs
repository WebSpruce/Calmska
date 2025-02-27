using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.DTO
{
    public class MoodDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("moodid")]
        public Guid? MoodId { get; set; }
        [JsonPropertyName("moodname")]
        public string? MoodName { get; set; } = string.Empty;
        [JsonPropertyName("moodtypeid")]
        public int MoodTypeId { get; set; } = 0;
    }
}

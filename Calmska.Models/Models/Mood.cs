using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class Mood
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("moodid")]
        public Guid MoodId { get; set; }
        [JsonPropertyName("moodname")]
        public required string MoodName { get; set; } = string.Empty;
        [JsonPropertyName("moodtypeid")]
        public required int MoodTypeId { get; set; } = 0;
        public Mood()
        {
            MoodId = Guid.NewGuid();
        }
    }
}

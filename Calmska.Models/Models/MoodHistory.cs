using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class MoodHistory
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("moodhistoryid")]
        public Guid MoodHistoryId { get; set; }
        [JsonPropertyName("date")]
        public required DateTime Date { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("userid")]
        public required Guid UserId { get; set; }
        [JsonPropertyName("moodid")]
        public required Guid MoodId { get; set; }
        public MoodHistory()
        {
            MoodHistoryId = Guid.NewGuid();
        }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Api.DTO
{
    public class MoodHistoryDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("moodhistoryid")]
        public Guid? MoodHistoryId { get; set; }
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }
        [JsonPropertyName("userid")]
        public Guid? UserId { get; set; }
        [JsonPropertyName("moodid")]
        public Guid? MoodId { get; set; }
    }
}

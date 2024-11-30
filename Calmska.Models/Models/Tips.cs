using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class Tips
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("tipid")]
        public Guid TipId { get; set; }
        [JsonPropertyName("content")]
        public required string Content { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public required string Type { get; set; } = string.Empty;
        public Tips()
        {
            TipId = Guid.NewGuid();
        }
    }
}

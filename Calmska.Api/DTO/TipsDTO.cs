using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Api.DTO
{
    public class TipsDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("tipid")]
        public Guid? TipId { get; set; }
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

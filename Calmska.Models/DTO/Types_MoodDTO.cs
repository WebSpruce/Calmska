using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.DTO
{
    public class Types_MoodDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("typeid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? TypeId { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

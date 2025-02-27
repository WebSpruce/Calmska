using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class Types_Tips
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("typeid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeId { get; set; }
        [JsonPropertyName("type")]
        public required string Type { get; set; } = string.Empty;
    }
}

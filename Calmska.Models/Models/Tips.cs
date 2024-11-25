using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Calmska.Models.Models
{
    public class Tips
    {
        [Key]
        [BsonElement("_id")]
        public Guid TipId { get; set; }
        public required string Content { get; set; } = string.Empty;
        public required string Type { get; set; } = string.Empty;
        public Tips()
        {
            TipId = Guid.NewGuid();
        }
    }
}

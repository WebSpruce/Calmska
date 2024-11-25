using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Calmska.Models.Models
{
    public class Mood
    {
        [Key]
        [BsonElement("_id")]
        public Guid MoodId { get; set; }
        public required string MoodName { get; set; } = string.Empty;
        public required string Type { get; set; } = string.Empty;
        public Mood()
        {
            MoodId = Guid.NewGuid();
        }
    }
}

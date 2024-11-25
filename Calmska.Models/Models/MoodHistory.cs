using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Calmska.Models.Models
{
    public class MoodHistory
    {
        [Key]
        [BsonElement("_id")]
        public Guid MoodHistoryId { get; set; }
        public required DateTime Date { get; set; }
        public required Guid UserId { get; set; }
        public required Guid MoodId { get; set; }
        public MoodHistory()
        {
            MoodHistoryId = Guid.NewGuid();
        }
    }
}

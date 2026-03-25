using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class MoodHistoryDocument
{
    [BsonElement("_id")]
    public Guid MoodHistoryId { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public Guid MoodId { get; set; }
}
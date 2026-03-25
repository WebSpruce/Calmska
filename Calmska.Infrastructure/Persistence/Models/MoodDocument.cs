using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class MoodDocument
{
    [BsonElement("_id")]
    public Guid MoodId { get; set; }
    public string? MoodName { get; set; } = string.Empty;
    public int MoodTypeId { get; set; }
}
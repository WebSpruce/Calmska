using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class Types_TipsDocument
{
    [BsonElement("_id")]
    public int TypeId { get; set; }
    public string Type { get; set; }
}
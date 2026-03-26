using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class TipsDocument
{
    [Key]
    [BsonElement("_id")]
    public Guid TipId { get; set; }
    public string Content { get; set; }
    public int TipsTypeId { get; set; }
}
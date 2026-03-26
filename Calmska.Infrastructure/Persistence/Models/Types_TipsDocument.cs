using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class Types_TipsDocument
{
    [Key]
    [BsonElement("_id")]
    public int TypeId { get; set; }
    public string Type { get; set; }
}
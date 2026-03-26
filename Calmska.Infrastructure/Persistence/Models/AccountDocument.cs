using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class AccountDocument
{
    [Key]
    [BsonElement("_id")]
    public Guid? UserId { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PasswordHashed { get; set; }
}
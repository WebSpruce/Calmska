using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Models.Models
{
    public class Account
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }
        [JsonPropertyName("username")]
        public required string UserName { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public required string Email { get; set; } = string.Empty;
        [JsonPropertyName("passwordhashed")]
        public required string PasswordHashed { get; set; } = string.Empty;
        public Account()
        {
            UserId = Guid.NewGuid();
        }
    }
}

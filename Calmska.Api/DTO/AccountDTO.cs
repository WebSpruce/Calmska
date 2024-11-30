using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Api.DTO
{
    public class AccountDTO
    {
        [Key]
        [BsonElement("_id")]
        [JsonPropertyName("userid")]
        public Guid? UserId { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string? Email { get; set; } = string.Empty;
        [JsonPropertyName("passwordhashed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PasswordHashed { get; set; }
    }
}

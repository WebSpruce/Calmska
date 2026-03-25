using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class AccountDTO
    {
        [Key]
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

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class Types_TipsDTO
    {
        [Key]
        [JsonPropertyName("typeid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? TypeId { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}

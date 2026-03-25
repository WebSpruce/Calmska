using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class TipsDTO
    {
        [Key]
        [JsonPropertyName("tipid")]
        public Guid? TipId { get; set; }
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("tipstypeid")]
        public int? TipsTypeId { get; set; }
    }
}

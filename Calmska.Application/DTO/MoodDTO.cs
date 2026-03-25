using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class MoodDTO
    {
        [Key]
        [JsonPropertyName("moodid")]
        public Guid? MoodId { get; set; }
        [JsonPropertyName("moodname")]
        public string? MoodName { get; set; } = string.Empty;
        [JsonPropertyName("moodtypeid")]
        public int MoodTypeId { get; set; } = 0;
    }
}

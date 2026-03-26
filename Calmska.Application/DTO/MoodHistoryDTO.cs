using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calmska.Application.DTO
{
    public class MoodHistoryDTO
    {
        [Key]
        [JsonPropertyName("moodhistoryid")]
        public Guid? MoodHistoryId { get; set; }
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }
        [JsonPropertyName("userid")]
        public Guid? UserId { get; set; }
        [JsonPropertyName("moodid")]
        public Guid? MoodId { get; set; }
    }
}

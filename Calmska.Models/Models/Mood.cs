namespace Calmska.Models.Models
{
    public class Mood
    {
        public Guid MoodId { get; set; }
        public required string MoodName { get; set; } = string.Empty;
        public required string Type { get; set; } = string.Empty;
    }
}

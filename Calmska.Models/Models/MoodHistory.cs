namespace Calmska.Models.Models
{
    public class MoodHistory
    {
        public Guid MoodHistoryId { get; set; }
        public required DateTime Date { get; set; }
        public required Guid UserId { get; set; }
        public required Guid MoodId { get; set; }
    }
}

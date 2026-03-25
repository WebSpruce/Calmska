namespace Calmska.Domain.Entities
{
    public class MoodHistory
    {
        public Guid MoodHistoryId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public Guid MoodId { get; set; }
        public MoodHistory()
        {
            MoodHistoryId = Guid.NewGuid();
        }
    }
}

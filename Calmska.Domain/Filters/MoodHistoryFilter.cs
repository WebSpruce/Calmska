namespace Calmska.Domain.Filters;

public class MoodHistoryFilter
{
    public Guid? MoodHistoryId { get; set; }
    public DateTime? Date { get; set; }
    public Guid? UserId { get; set; }
    public Guid? MoodId { get; set; }

    public MoodHistoryFilter(Guid? moodHistoryId, DateTime? date, Guid? userId, Guid? moodId)
    {
        MoodHistoryId = moodHistoryId;
        Date = date;
        UserId = userId;
        MoodId = moodId;
    }
}
namespace Calmska.Domain.Filters;

public class MoodFilter
{
    public Guid? MoodId { get; set; }
    public string? MoodName { get; set; }
    public int? MoodTypeId { get; set; }
    
    public MoodFilter(Guid? moodId, string? moodName, int? moodTypeId)
    {
        MoodId = moodId;
        MoodName = moodName;
        MoodTypeId = moodTypeId;
    }
}
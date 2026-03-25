namespace Calmska.Domain.Entities
{
    public class Mood
    {
        public Guid MoodId { get; set; }
        public string MoodName { get; set; } = string.Empty;
        public int MoodTypeId { get; set; }
        public Mood()
        {
            MoodId = Guid.NewGuid();
        }
    }
}

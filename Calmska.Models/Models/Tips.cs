namespace Calmska.Models.Models
{
    public class Tips
    {
        public Guid TipId { get; set; }
        public required string Content { get; set; } = string.Empty;
        public required string Type { get; set; } = string.Empty;
    }
}

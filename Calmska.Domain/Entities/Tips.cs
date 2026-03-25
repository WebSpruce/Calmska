namespace Calmska.Domain.Entities
{
    public class Tips
    {
        public Guid TipId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TipsTypeId { get; set; }
        public Tips()
        {
            TipId = Guid.NewGuid();
        }
    }
}

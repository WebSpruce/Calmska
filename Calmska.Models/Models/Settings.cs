namespace Calmska.Models.Models
{
    public class Settings
    {
        public Guid SettingsId { get; set; }
        public required string SettingsJson { get; set; } = string.Empty;
        public required Guid UserId { get; set; }
    }
}

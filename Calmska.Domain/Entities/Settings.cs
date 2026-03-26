namespace Calmska.Domain.Entities
{
    public class Settings
    {
        public Guid SettingsId { get; set; }
        public string Color { get; set; } = string.Empty;
        public string PomodoroTimer { get; set; } = string.Empty;
        public string PomodoroBreak { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Settings()
        {
            SettingsId = Guid.NewGuid();
        }
    }
}

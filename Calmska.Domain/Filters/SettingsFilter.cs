namespace Calmska.Domain.Filters;

public class SettingsFilter
{
    public Guid? SettingsId { get; set; }
    public string? Color { get; set; }
    public string? PomodoroTimer { get; set; }
    public string? PomodoroBreak { get; set; }
    public Guid? UserId { get; set; }

    public SettingsFilter(Guid? settingsId, string? color, string? pomodoroTimer, string? pomodoroBreak, Guid? userId)
    {
        SettingsId = settingsId;
        Color = color;
        PomodoroTimer = pomodoroTimer;
        PomodoroBreak = pomodoroBreak;
        UserId = userId;
    }
}
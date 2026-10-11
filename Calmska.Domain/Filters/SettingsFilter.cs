namespace Calmska.Domain.Filters;

public class SettingsFilter
{
    public Guid? SettingsId { get; set; }
    public string? Color { get; set; }
    public float? PomodoroTimer { get; set; }
    public float? PomodoroBreak { get; set; }
    public Guid? UserId { get; set; }

    public SettingsFilter(Guid? settingsId, string? color, float? pomodoroTimer, float? pomodoroBreak, Guid? userId)
    {
        SettingsId = settingsId;
        Color = color;
        PomodoroTimer = pomodoroTimer;
        PomodoroBreak = pomodoroBreak;
        UserId = userId;
    }
}
using MongoDB.Bson.Serialization.Attributes;

namespace Calmska.Infrastructure.Persistence.Models;

public class SettingsDocument
{
    [BsonElement("_id")]
    public Guid SettingsId { get; set; }
    public string? Color { get; set; } = string.Empty;
    public string? PomodoroTimer { get; set; } = string.Empty;
    public string? PomodoroBreak { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
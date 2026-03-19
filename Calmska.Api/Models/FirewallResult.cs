namespace Calmska.Api.Models;

public class FirewallResult
{
    public bool IsBlocked { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string SanitizedContent { get; set; } = string.Empty;
}
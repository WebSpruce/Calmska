using Calmska.Api.Models;

namespace Calmska.Api.Interfaces;

public interface IAiFirewallService
{
    Task<FirewallResult> InspectInputAsync(string userPrompt, CancellationToken token);
    Task<FirewallResult> InspectOutputAsync(string llmResponse, string originalPrompt, CancellationToken token);
}
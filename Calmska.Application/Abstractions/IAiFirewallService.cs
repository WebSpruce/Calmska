using Calmska.Application.DTO;

namespace Calmska.Application.Abstractions;

public interface IAiFirewallService
{
    Task<FirewallResult> InspectInputAsync(string userPrompt, CancellationToken token);
    Task<FirewallResult> InspectOutputAsync(string llmResponse, string originalPrompt, CancellationToken token);
}
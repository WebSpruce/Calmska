using Calmska.Application.DTO;

namespace Calmska.ApiClients.Interfaces;

public interface IAiPromptingApiClient
{
    Task<string> GetPromptResponseAsync(PromptRequest request, CancellationToken token);
}
using Calmska.Application.DTO;

namespace Calmska.Interfaces;

public interface IAiPromptingApiClient
{
    Task<string> GetPromptResponseAsync(PromptRequest request, CancellationToken token);
}
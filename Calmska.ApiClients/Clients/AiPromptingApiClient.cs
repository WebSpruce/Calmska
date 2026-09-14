using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Microsoft.Maui.Controls;

namespace Calmska.ApiClients.Clients;

public class AiPromptingApiClient : IAiPromptingApiClient
{
    private readonly IHttpClientService _httpClientService;

    public AiPromptingApiClient(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<string> GetPromptResponseAsync(PromptRequest request, CancellationToken token)
    {
        try
        {
            var result = await _httpClientService.PostAsync<PromptRequest, string>("prompts/prompt", request, token);

            if (!string.IsNullOrEmpty(result.Result) && string.IsNullOrEmpty(result.Error))
            {
                return result.Result;
            }
            return $"AI service error: {result.Error}";
        }
        catch(Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Warning", $"Send prompt request error.", "Close");
            return string.Empty;
        }
    }
}
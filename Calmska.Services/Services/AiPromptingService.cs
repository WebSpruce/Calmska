using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;

namespace Calmska.Services.Services;

public class AiPromptingService : IAiPromptingService
{
    private readonly HttpClient _httpClient;

    public AiPromptingService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetPromptResponseAsync(PromptRequest request, CancellationToken token)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("prompts/prompt", request, token);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OperationResultT<string>>(token);
            return result?.Result ?? string.Empty;
        }
        catch(Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Warning", $"Send prompt request error.", "Close");
            return string.Empty;
        }
    }
}
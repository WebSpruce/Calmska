using System.Net.Http.Json;
using System.Text.Json;
using Calmska.Application.Abstractions;
using Calmska.Application.DTO;
using Microsoft.Extensions.Options;

namespace Calmska.Infrastructure.Persistence.AI;

public class AiFirewallService : IAiFirewallService
{
    private readonly HttpClient _httpClient;
    private readonly AiPromptingOptions _options;
    private readonly string _inputClassifierPrompt;
    private readonly string _outputClassifierPrompt;

    public AiFirewallService(HttpClient httpClient, IOptions<AiPromptingOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        
        _inputClassifierPrompt  = PromptLoader.Load("input_classifier.json");
        _outputClassifierPrompt = PromptLoader.Load("output_classifier.json");
    }

    public async Task<FirewallResult> InspectInputAsync(string userPrompt, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
            return new FirewallResult { IsBlocked = true, Reason = "Empty prompt." };

        if (userPrompt.Length > 2000)
            return new FirewallResult { IsBlocked = true, Reason = "Prompt too long." };

        var classifierResponse = await CallClassifierAsync(
            _inputClassifierPrompt,
            $"Classify this user input: \"{userPrompt}\"",
            token);

        // ignore any extra LLM output
        var verdict = classifierResponse.Trim().Split(' ', '\n')[0].TrimEnd('.').ToUpper();

        if (verdict == "INJECTION")
            return new FirewallResult
            {
                IsBlocked = true,
                Reason = "Prompt injection detected by AI classifier."
            };

        return new FirewallResult
        {
            IsBlocked = false,
            SanitizedContent = userPrompt.Trim()
        };
    }

    public async Task<FirewallResult> InspectOutputAsync(
        string llmResponse, string originalPrompt, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(llmResponse))
            return new FirewallResult { IsBlocked = true, Reason = "Empty LLM response." };

        var classifierResponse = await CallClassifierAsync(
            _outputClassifierPrompt,
            $"Original user request: \"{originalPrompt}\"\nAI response to validate: \"{llmResponse}\"",
            token);

        // ignore any extra LLM output
        var verdict = classifierResponse.Trim().Split(' ', '\n')[0].TrimEnd('.').ToUpper();

        if (verdict == "UNSAFE")
            return new FirewallResult
            {
                IsBlocked = true,
                Reason = "LLM response flagged as unsafe by AI classifier."
            };

        return new FirewallResult
        {
            IsBlocked = false,
            SanitizedContent = llmResponse.Trim()
        };
    }

    private async Task<string> CallClassifierAsync(
        string systemPrompt, string userMessage, CancellationToken token)
    {
        var requestData = new
        {
            model = _options.ApiModel,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userMessage }
            },
            max_completion_tokens = 5,
            temperature = 0
        };
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_options.ApiHost}chat/completions"),
            Headers =
            {
                { "Authorization", $"Bearer {_options.ApiKey}" }
            },
            Content = JsonContent.Create(requestData)
        };
        return await SendRequestAsync(request, _httpClient);
    }
    
    private async Task<string> SendRequestAsync(HttpRequestMessage request, HttpClient client)
    {
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<JsonElement>(body);
                
            string resultValue = responseObj
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            resultValue = resultValue.Trim().Split(' ', '\n')[0].TrimEnd('.').ToUpper();
                
            return resultValue ?? string.Empty;
        }
    }
}
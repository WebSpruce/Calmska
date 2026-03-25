using System.Net.Http.Json;
using System.Text.Json;
using Calmska.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Calmska.Infrastructure.Persistence.AI;

public class AiPromptingRepository : IAiPromptingRepository
{
    private readonly HttpClient _httpClient;
    private readonly AiPromptingOptions _options;
    private readonly IAiFirewallService _firewall;

    public AiPromptingRepository(IConfiguration config, HttpClient httpClient, IOptions<AiPromptingOptions> options, IAiFirewallService firewall)
    {
        _httpClient = httpClient;
        _firewall = firewall;
        _options = options.Value;
    }
    public async Task<string> GetPromptResponseAsync(string prompt, bool isAnalize, bool isMoodEntry, CancellationToken token)
    {
        // prevent prompt injection in input
        if (!isAnalize)
        { 
            var inputCheck = await _firewall.InspectInputAsync(prompt, token);
            if(inputCheck.IsBlocked)
                return $"Request blocked: {inputCheck.Reason}";
            
            if (isMoodEntry)
            {
                prompt =
                    $"User's description of their current mood: \"{prompt}\". Based on the user's description of their current mood, select the most appropriate mood name from the following list: 'Inspired, Grateful, Curious, Worried, Anxious, Lonely, Bored, Indifferent, Accepting, Resentful, Determined, Peaceful, Overwhelmed, Content, Pensive, Reserved, Reflective, Hopeless, Calm, Confident, Proud, Frustrated, Insecure, Tranquil, Nostalgic, Ecstatic, Cheerful, Energized, Jealous, Irritated, Guilty, Serene, Disappointed, Melancholic, Neutral.' Respond with only one word from this list and nothing else.";
            }
        }
        else
        {
            prompt =
                $"You are an empathetic and insightful AI assistant specializing in mood analysis and well-being. The user has entered their mood for several days in the app. Your tasks are:Analyze the mood data:Identify patterns, trends, or fluctuations in the user's mood over the recorded period.Note any significant changes, recurring themes, or potential triggers (if mentioned).Provide supportive feedback:Summarize your observations in a gentle, encouraging, and non-judgmental tone.Acknowledge positive moments and empathize with any challenges.Offer practical, personalized advice:Suggest specific, actionable steps inspired by the concept of “hygge” (coziness, comfort, connection, simple pleasures).Tailor your advice to the user’s mood patterns and any details they’ve shared.Include ideas for daily rituals, environment adjustments, social connections, or mindful activities that promote a hygge lifestyle. User's moods:{prompt}. Don't write additional content just write advices. Write up to 100 words.";
        }
        
        var requestData = new
        {
            model = _options.ApiModel,
            messages = new[]
            {
                new { role = "user",   content = prompt }
            },
            temperature = isAnalize ? 0.7 : 0,
            max_completion_tokens = 1024
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
        var response = await SendRequestAsync(request, _httpClient);
        
        // prevent prompt injection in output
        if (!isAnalize)
        {
            var outputCheck = await _firewall.InspectOutputAsync(response, prompt, token);
            if (outputCheck.IsBlocked)
                return $"Response blocked: {outputCheck.Reason}";
        }

        return response;
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

            if (resultValue.EndsWith("."))
                resultValue = resultValue.Replace(".", "");
                
            return resultValue ?? string.Empty;
        }
    }
}
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
#if ANDROID
using Android.Util;
#endif

namespace Calmska.Helper;

public static class Prompting
{
    internal static async Task<string> SendPromptRequest(string prompt)
    {
        try
        {
            var client = new HttpClient();
            var requestData = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                web_access = false
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://chatgpt-42.p.rapidapi.com/gpt4"),
                Headers =
                {
                    { "x-rapidapi-key", "65ba63e3e0msh913a750835be7a3p1258cejsn2aba8caa2766" },
                    { "x-rapidapi-host", "chatgpt-42.p.rapidapi.com" },
                },
                Content = JsonContent.Create(requestData)
            };
            #if ANDROID
            Log.Debug("Prompt", $"request prompt: {prompt}");
            #endif
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"response: {body}");
                var responseObj = JsonSerializer.Deserialize<JsonElement>(body);
                string resultValue = responseObj.GetProperty("result").GetString();
                if (resultValue.EndsWith("."))
                    resultValue = resultValue.Replace(".", "");
                return resultValue ?? string.Empty;
            }
        }
        catch(Exception ex)
        {
            await Shell.Current.DisplayAlert("Warning", $"Send prompt request error.", "Close");
            return string.Empty;
        }
    }
}
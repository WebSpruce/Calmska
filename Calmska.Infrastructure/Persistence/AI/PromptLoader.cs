using System.Text.Json;

namespace Calmska.Infrastructure.Persistence.AI;

public static class PromptLoader
{
    private static readonly string _promptsPath = 
        Path.Combine(AppContext.BaseDirectory, "Prompts");

    private static readonly Dictionary<string, string> _cache = new();

    public static string Load(string fileName)
    {
        if (_cache.TryGetValue(fileName, out var cached))
            return cached;

        var filePath = Path.Combine(_promptsPath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Prompt file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        var document = JsonDocument.Parse(json);
        var prompt = document.RootElement
                         .GetProperty("systemPrompt")
                         .GetString()
                     ?? throw new InvalidOperationException(
                         $"'systemPrompt' key missing in {fileName}");

        _cache[fileName] = prompt;
        return prompt;
    }
    public static void ClearCache() => _cache.Clear();
}

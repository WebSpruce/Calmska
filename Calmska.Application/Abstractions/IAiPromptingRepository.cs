namespace Calmska.Application.Abstractions;

public interface IAiPromptingRepository
{
    Task<string> GetPromptResponseAsync(string prompt, bool isAnalize, bool isMoodEntry, CancellationToken token);
}
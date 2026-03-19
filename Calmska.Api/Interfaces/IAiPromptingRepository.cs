namespace Calmska.Api.Interfaces;

public interface IAiPromptingRepository
{
    Task<string> GetPromptResponseAsync(string prompt, bool isAnalize, bool isMoodEntry, CancellationToken token);
}
using System.Threading;
using System.Threading.Tasks;
using Calmska.Models.Models;

namespace Calmska.Services.Interfaces;

public interface IAiPromptingService
{
    Task<string> GetPromptResponseAsync(PromptRequest request, CancellationToken token);
}
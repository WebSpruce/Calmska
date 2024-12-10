using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IMoodRepository
    {
        Task<IEnumerable<Mood>> GetAllAsync();
        Task<IEnumerable<Mood>> GetAllByArgumentAsync(MoodDTO moodDTO);
        Task<Mood?> GetByArgumentAsync(MoodDTO mood);
        Task<OperationResult> AddAsync(MoodDTO mood);
        Task<OperationResult> UpdateAsync(MoodDTO mood);
        Task<OperationResult> DeleteAsync(Guid moodId);
    }
}

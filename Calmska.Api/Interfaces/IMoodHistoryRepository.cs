using Calmska.Api.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IMoodHistoryRepository
    {
        Task<IEnumerable<MoodHistory>> GetAllAsync();
        Task<IEnumerable<MoodHistory>> GetAllByArgumentAsync(MoodHistoryDTO moodHistoryDTO);
        Task<MoodHistory?> GetByArgumentAsync(MoodHistoryDTO moodHistoryDTO);
        Task<OperationResult> AddAsync(MoodHistoryDTO moodHistoryDTO);
        Task<OperationResult> UpdateAsync(MoodHistoryDTO moodHistoryDTO);
        Task<OperationResult> DeleteAsync(Guid moodHistoryId);
    }
}

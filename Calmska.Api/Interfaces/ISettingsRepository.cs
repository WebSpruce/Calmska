using Calmska.Api.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface ISettingsRepository
    {
        Task<IEnumerable<Settings>> GetAllAsync();
        Task<IEnumerable<Settings>> GetAllByArgumentAsync(Settings account);
        Task<Settings?> GetByArgumentAsync(Settings account);
        Task<OperationResult> AddAsync(Settings account);
        Task<OperationResult> UpdateAsync(Settings account);
        Task<OperationResult> DeleteAsync(Settings account);
    }
}

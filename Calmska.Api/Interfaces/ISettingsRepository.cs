using Calmska.Api.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface ISettingsRepository
    {
        Task<IEnumerable<Settings>> GetAllAsync();
        Task<IEnumerable<Settings>> GetAllByArgumentAsync(SettingsDTO settingsDTO);
        Task<Settings?> GetByArgumentAsync(SettingsDTO settingsDTO);
        Task<OperationResult> AddAsync(SettingsDTO settingsDTO);
        Task<OperationResult> UpdateAsync(SettingsDTO settingsDTO);
        Task<OperationResult> DeleteAsync(Guid settingsId);
    }
}

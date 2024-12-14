using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface ITipsRepository
    {
        Task<IEnumerable<Tips>> GetAllAsync();
        Task<IEnumerable<Tips>> GetAllByArgumentAsync(TipsDTO tipsDTO);
        Task<Tips?> GetByArgumentAsync(TipsDTO tipsDTO);
        Task<OperationResult> AddAsync(TipsDTO tipsDTO);
        Task<OperationResult> UpdateAsync(TipsDTO tipsDTO);
        Task<OperationResult> DeleteAsync(Guid tipsId);
    }
}

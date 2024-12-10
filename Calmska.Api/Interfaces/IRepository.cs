using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IRepository<TEntity, TFilter>
    {
        Task<PaginatedResult<TEntity>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PaginatedResult<TEntity>> GetAllByArgumentAsync(TFilter filter, int? pageNumber, int? pageSize);
        Task<TEntity?> GetByArgumentAsync(TFilter filter);
        Task<OperationResult> AddAsync(TFilter entity);
        Task<OperationResult> UpdateAsync(TFilter entity);
        Task<OperationResult> DeleteAsync(Guid id);
    }
}

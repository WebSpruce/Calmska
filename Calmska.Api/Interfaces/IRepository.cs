using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface IRepository<TEntity, TFilter>
    {
        Task<PaginatedResult<TEntity>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token);
        Task<PaginatedResult<TEntity>> GetAllByArgumentAsync(TFilter filter, int? pageNumber, int? pageSize, CancellationToken token);
        Task<TEntity?> GetByArgumentAsync(TFilter filter, CancellationToken token);
        Task<OperationResult> AddAsync(TFilter entity, CancellationToken token);
        Task<OperationResult> UpdateAsync(TFilter entity, CancellationToken token);
        Task<OperationResult> DeleteAsync(Guid id, CancellationToken token);
    }
}

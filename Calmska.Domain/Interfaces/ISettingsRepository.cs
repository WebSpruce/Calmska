using Calmska.Domain.Common;

namespace Calmska.Domain.Interfaces;

public interface ISettingsRepository<TEntity, TDto, TFilter>
{
    Task<PaginatedResult<TEntity>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token);
    Task<PaginatedResult<TEntity>> GetAllByArgumentAsync(TFilter filter, int? pageNumber, int? pageSize, CancellationToken token);
    Task<TDto?> GetByArgumentAsync(TFilter filter, CancellationToken token);
    Task<OperationResult> AddAsync(TEntity entity, CancellationToken token);
    Task<OperationResult> UpdateAsync(TFilter entity, CancellationToken token);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken token);
}
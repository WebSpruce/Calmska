using Calmska.Models.Models;

namespace Calmska.Api.Interfaces
{
    public interface ITypesRepository<TEntity, TFilter>
    {
        Task<PaginatedResult<TEntity>> GetAllAsync(int? pageNumber, int? pageSize);
        Task<PaginatedResult<TEntity>> GetAllByArgumentAsync(TFilter filter, int? pageNumber, int? pageSize);
        Task<TEntity?> GetByArgumentAsync(TFilter filter);
        Task<OperationResult> AddAsync(TFilter entity);
        Task<OperationResult> UpdateAsync(TFilter entity);
        Task<OperationResult> DeleteAsync(int id);
    }
}

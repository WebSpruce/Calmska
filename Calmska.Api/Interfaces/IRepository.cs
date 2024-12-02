using Calmska.Api.DTO;

namespace Calmska.Api.Interfaces
{
    public interface IRepository<TEntity, TFilter>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllByArgumentAsync(TFilter filter);
        Task<TEntity?> GetByArgumentAsync(TFilter filter);
        Task<OperationResult> AddAsync(TFilter entity);
        Task<OperationResult> UpdateAsync(TFilter entity);
        Task<OperationResult> DeleteAsync(Guid id);
    }
}

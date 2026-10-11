using Calmska.Domain.Common;

namespace Calmska.ApiClients.Interfaces;

public interface ISettingsApiClient<TDto>
{
        Task<OperationResultT<PaginatedResult<TDto?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token);
        Task<OperationResultT<PaginatedResult<TDto?>>> SearchAllByArgumentAsync(TDto criteria, int? pageNumber, int? pageSize, CancellationToken token);
        Task<OperationResultT<TDto?>> GetByArgumentAsync(TDto criteria, CancellationToken token);
        Task<OperationResultT<bool>> AddAsync(TDto newObject, CancellationToken token);
        Task<OperationResultT<bool>> UpdateAsync(TDto updatedObject, CancellationToken token);
        Task<OperationResultT<bool>> DeleteAsync(Guid objectId, CancellationToken token);
}
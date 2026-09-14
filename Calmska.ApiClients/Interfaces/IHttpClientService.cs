using Calmska.Domain.Common;

namespace Calmska.ApiClients.Interfaces;

public interface IHttpClientService
{
    Task<OperationResultT<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<OperationResultT<bool>> PostAsync<T>(string endpoint, T data, CancellationToken cancellationToken = default);
    Task<OperationResultT<bool>> PutAsync<T>(string endpoint, T data, CancellationToken cancellationToken = default);
    Task<OperationResultT<bool>> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}
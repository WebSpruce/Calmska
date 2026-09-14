using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class TipsApiClient : IApiClient<TipsDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public TipsApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<TipsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<TipsDTO?>>("tips", token);
        }

        public async Task<OperationResultT<PaginatedResult<TipsDTO?>>> SearchAllByArgumentAsync(TipsDTO tipsCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(tipsCriteria);
            string endpoint = $"tips/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<TipsDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<TipsDTO?>> GetByArgumentAsync(TipsDTO tipsCriteria, CancellationToken token)
        {
            var query = BuildQueryString(tipsCriteria);
            string endpoint = $"tips/search?{query}";

            return await _httpClientService.GetAsync<TipsDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(TipsDTO newTip, CancellationToken token)
        {
            return await _httpClientService.PostAsync<TipsDTO?>("tips", newTip, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(TipsDTO updatedTip, CancellationToken token)
        {
            return await _httpClientService.PutAsync<TipsDTO?>("tips", updatedTip, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid tipId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"tips?tipId={tipId}", token);
        }
        
        private static string BuildQueryString(TipsDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.TipId.HasValue)
                parameters.Add($"TipId={criteria.TipId}");
            if (!string.IsNullOrEmpty(criteria.Content))
                parameters.Add($"Content={Uri.EscapeDataString(criteria.Content)}");
            if (criteria.TipsTypeId != null)
                parameters.Add($"Type={criteria.TipsTypeId}");
            
            return string.Join("&", parameters);
        }
    }
}

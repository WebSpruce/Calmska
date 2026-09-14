using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class TypesTipsApiClient : ITypesApiClient<Types_TipsDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public TypesTipsApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<Types_TipsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<Types_TipsDTO?>>("types_tips", token);
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_TipsDTO?>>>> SearchAllByArgumentAsync(Types_TipsDTO typesCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(typesCriteria);
            string endpoint = $"types_tips/searchList?{query}"; 

            return await _httpClientService.GetAsync<PaginatedResult<IEnumerable<Types_TipsDTO?>>>(endpoint, token);
        }

        public async Task<OperationResultT<Types_TipsDTO?>> GetByArgumentAsync(Types_TipsDTO typesCriteria, CancellationToken token)
        {
            var query = BuildQueryString(typesCriteria);
            string endpoint = $"types_tips/search?{query}"; 

            return await _httpClientService.GetAsync<Types_TipsDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_TipsDTO newTip, CancellationToken token)
        {
            return await _httpClientService.PostAsync<Types_TipsDTO?>("types_tips", newTip, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_TipsDTO updatedTip, CancellationToken token)
        {
            return await _httpClientService.PutAsync<Types_TipsDTO?>("types_tips", updatedTip, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"types_tips?TypeId={TypeId}", token);
        }
        
        private static string BuildQueryString(Types_TipsDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.TypeId.HasValue)
                parameters.Add($"TypeId={criteria.TypeId}");
            if (!string.IsNullOrEmpty(criteria.Type))
                parameters.Add($"Type={Uri.EscapeDataString(criteria.Type)}");
            
            return string.Join("&", parameters);
        }
    }
}

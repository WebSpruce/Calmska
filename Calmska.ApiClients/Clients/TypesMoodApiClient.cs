using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class TypesMoodApiClient : ITypesApiClient<Types_MoodDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public TypesMoodApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<Types_MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<Types_MoodDTO?>>("types_moods", token);
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_MoodDTO?>>>> SearchAllByArgumentAsync(Types_MoodDTO typesCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(typesCriteria);
            string endpoint = $"types_moods/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<IEnumerable<Types_MoodDTO?>>>(endpoint, token);
        }

        public async Task<OperationResultT<Types_MoodDTO?>> GetByArgumentAsync(Types_MoodDTO typesCriteria, CancellationToken token)
        {
            var query = BuildQueryString(typesCriteria);
            string endpoint = $"types_moods/search?{query}";

            return await _httpClientService.GetAsync<Types_MoodDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_MoodDTO newTip, CancellationToken token)
        {
            return await _httpClientService.PostAsync<Types_MoodDTO?>("types_moods", newTip, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_MoodDTO updatedTip, CancellationToken token)
        {
            return await _httpClientService.PutAsync<Types_MoodDTO?>("types_moods", updatedTip, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"types_moods/{TypeId:D}", token);
        }
        
        private static string BuildQueryString(Types_MoodDTO criteria)
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

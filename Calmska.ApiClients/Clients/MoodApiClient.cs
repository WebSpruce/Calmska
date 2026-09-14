using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class MoodApiClient : IApiClient<MoodDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public MoodApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<MoodDTO?>>("moods", token);
        }

        public async Task<OperationResultT<PaginatedResult<MoodDTO?>>> SearchAllByArgumentAsync(MoodDTO moodCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(moodCriteria);
            string endpoint = $"moods/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<MoodDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<MoodDTO?>> GetByArgumentAsync(MoodDTO moodCriteria, CancellationToken token)
        {
            var query = BuildQueryString(moodCriteria);
            string endpoint = $"moods/search?{query}";

            return await _httpClientService.GetAsync<MoodDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodDTO newAccount, CancellationToken token)
        {
            return await _httpClientService.PostAsync<MoodDTO?>("moods", newAccount, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodDTO updatedAccount, CancellationToken token)
        {
            return await _httpClientService.PutAsync<MoodDTO?>("moods", updatedAccount, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"moods?moodId={moodId}", token);
        }
        
        private static string BuildQueryString(MoodDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.MoodId.HasValue)
                parameters.Add($"MoodId={criteria.MoodId}");
            if (!string.IsNullOrEmpty(criteria.MoodName))
                parameters.Add($"MoodName={Uri.EscapeDataString(criteria.MoodName)}");
            if (criteria.MoodTypeId != null)
                parameters.Add($"Type={criteria.MoodTypeId}");
            
            return string.Join("&", parameters);
        }
    }
}

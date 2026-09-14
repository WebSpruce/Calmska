using Calmska.ApiClients.Interfaces;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ApiClients.Clients
{
    public class MoodHistoryApiClient : IApiClient<MoodHistoryDTO>
    {
        private readonly IHttpClientService _httpClientService;
        public MoodHistoryApiClient(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OperationResultT<PaginatedResult<MoodHistoryDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await _httpClientService.GetAsync<PaginatedResult<MoodHistoryDTO?>>("moodhistory", token);
        }

        public async Task<OperationResultT<PaginatedResult<MoodHistoryDTO?>>> SearchAllByArgumentAsync(MoodHistoryDTO moodHistoryCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            var query = BuildQueryString(moodHistoryCriteria);
            string endpoint = $"moodhistory/searchList?{query}";

            return await _httpClientService.GetAsync<PaginatedResult<MoodHistoryDTO?>>(endpoint, token);
        }

        public async Task<OperationResultT<MoodHistoryDTO?>> GetByArgumentAsync(MoodHistoryDTO moodHistoryCriteria, CancellationToken token)
        {
            var query = BuildQueryString(moodHistoryCriteria);
            string endpoint = $"moodhistory/search?{query}";

            return await _httpClientService.GetAsync<MoodHistoryDTO?>(endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodHistoryDTO newMoodHistory, CancellationToken token)
        {
            return await _httpClientService.PostAsync<MoodHistoryDTO?>("moodhistory", newMoodHistory, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodHistoryDTO updatedMoodHistory, CancellationToken token)
        {
            return await _httpClientService.PutAsync<MoodHistoryDTO?>("moodhistory", updatedMoodHistory, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodHistoryId, CancellationToken token)
        {
            return await _httpClientService.DeleteAsync($"moodhistory?moodHistoryId={moodHistoryId}", token);
        }
        
        private static string BuildQueryString(MoodHistoryDTO criteria)
        {
            if (criteria is null) return string.Empty;

            var parameters = new List<string>();

            if (criteria.MoodHistoryId.HasValue)
                parameters.Add($"MoodHistoryId={criteria.MoodHistoryId}");
            if (criteria.Date.HasValue)
            {
                var formattedDate = Uri.EscapeDataString(criteria.Date.Value.ToString("o")); // ISO 8601
                parameters.Add($"Date={formattedDate}");
            }
            if (criteria.UserId.HasValue)
                parameters.Add($"UserId={criteria.UserId}");
            if (criteria.MoodId.HasValue)
                parameters.Add($"MoodId={criteria.MoodId}");
            
            return string.Join("&", parameters);
        }
    }
}

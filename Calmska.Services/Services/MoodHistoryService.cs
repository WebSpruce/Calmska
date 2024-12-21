using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Helper;
using Calmska.Services.Interfaces;

namespace Calmska.Services.Services
{
    public class MoodHistoryService : IService<MoodHistoryDTO>
    {
        private readonly HttpClient _httpClient;
        public MoodHistoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<IEnumerable<MoodHistoryDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<IEnumerable<MoodHistoryDTO?>>(_httpClient, "/moodhistory");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<MoodHistoryDTO?>>>> SearchAllByArgumentAsync(MoodHistoryDTO moodHistoryCriteria, int? pageNumber, int? pageSize)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (moodHistoryCriteria != null)
            {
                if (moodHistoryCriteria.MoodHistoryId.HasValue)
                {
                    endpointParameters.Add($"MoodHistoryId={moodHistoryCriteria.MoodHistoryId}");
                }
                if (moodHistoryCriteria.Date.HasValue)
                {
                    var formattedDate = Uri.EscapeDataString(moodHistoryCriteria.Date.Value.ToString("o")); // ISO 8601
                    endpointParameters.Add($"Date={formattedDate}");
                }
                if (moodHistoryCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={moodHistoryCriteria.UserId}");
                }
                if (moodHistoryCriteria.MoodId.HasValue)
                {
                    endpointParameters.Add($"MoodId={moodHistoryCriteria.MoodId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/moodhistory/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<MoodHistoryDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<MoodHistoryDTO?>> GetByArgumentAsync(MoodHistoryDTO moodHistoryCriteria)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (moodHistoryCriteria != null)
            {
                if (moodHistoryCriteria.MoodHistoryId.HasValue)
                {
                    endpointParameters.Add($"MoodHistoryId={moodHistoryCriteria.MoodHistoryId}");
                }
                if (moodHistoryCriteria.Date.HasValue)
                {
                    var formattedDate = Uri.EscapeDataString(moodHistoryCriteria.Date.Value.ToString("o")); // ISO 8601
                    endpointParameters.Add($"Date={formattedDate}");
                }
                if (moodHistoryCriteria.UserId.HasValue)
                {
                    endpointParameters.Add($"UserId={moodHistoryCriteria.UserId}");
                }
                if (moodHistoryCriteria.MoodId.HasValue)
                {
                    endpointParameters.Add($"MoodId={moodHistoryCriteria.MoodId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"/moodhistory/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<MoodHistoryDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodHistoryDTO newMoodHistory)
        {
            return await HttpClientHelper.PostAsync<MoodHistoryDTO?>(_httpClient, "/moodhistory", newMoodHistory);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodHistoryDTO updatedMoodHistory)
        {
            return await HttpClientHelper.PutAsync<MoodHistoryDTO?>(_httpClient, "/moodhistory", updatedMoodHistory);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodHistoryId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/moodhistory?moodHistoryId={moodHistoryId}");
        }
    }
}

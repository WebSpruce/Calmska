using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Calmska.Application.DTO;
using Calmska.Domain.Common;
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

        public async Task<OperationResultT<PaginatedResult<MoodHistoryDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<MoodHistoryDTO?>>(_httpClient, "moodhistory", token);
        }

        public async Task<OperationResultT<PaginatedResult<MoodHistoryDTO?>>> SearchAllByArgumentAsync(MoodHistoryDTO moodHistoryCriteria, int? pageNumber, int? pageSize, CancellationToken token)
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
                endpoint = $"moodhistory/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<MoodHistoryDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<MoodHistoryDTO?>> GetByArgumentAsync(MoodHistoryDTO moodHistoryCriteria, CancellationToken token)
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
                endpoint = $"moodhistory/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<MoodHistoryDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodHistoryDTO newMoodHistory, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<MoodHistoryDTO?>(_httpClient, "moodhistory", newMoodHistory, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodHistoryDTO updatedMoodHistory, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<MoodHistoryDTO?>(_httpClient, "moodhistory", updatedMoodHistory, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodHistoryId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"moodhistory?moodHistoryId={moodHistoryId}", token);
        }
    }
}

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
    public class MoodService : IService<MoodDTO>
    {
        private readonly HttpClient _httpClient;
        public MoodService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<MoodDTO?>>(_httpClient, "moods", token);
        }

        public async Task<OperationResultT<PaginatedResult<MoodDTO?>>> SearchAllByArgumentAsync(MoodDTO moodCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (moodCriteria != null)
            {
                if (moodCriteria.MoodId.HasValue)
                {
                    endpointParameters.Add($"MoodId={moodCriteria.MoodId}");
                }
                if (!string.IsNullOrEmpty(moodCriteria.MoodName))
                {
                    endpointParameters.Add($"MoodName={Uri.EscapeDataString(moodCriteria.MoodName)}");
                }
                if (moodCriteria.MoodTypeId != null)
                {
                    endpointParameters.Add($"Type={moodCriteria.MoodTypeId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"moods/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<MoodDTO?>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<MoodDTO?>> GetByArgumentAsync(MoodDTO moodCriteria, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (moodCriteria != null)
            {
                if (moodCriteria.MoodId.HasValue)
                {
                    endpointParameters.Add($"MoodId={moodCriteria.MoodId}");
                }
                if (!string.IsNullOrEmpty(moodCriteria.MoodName))
                {
                    endpointParameters.Add($"MoodName={Uri.EscapeDataString(moodCriteria.MoodName)}");
                }
                if (moodCriteria.MoodTypeId != null)
                {
                    endpointParameters.Add($"Type={moodCriteria.MoodTypeId}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"moods/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<MoodDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodDTO newAccount, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<MoodDTO?>(_httpClient, "moods", newAccount, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodDTO updatedAccount, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<MoodDTO?>(_httpClient, "moods", updatedAccount, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"moods?moodId={moodId}", token);
        }
    }
}

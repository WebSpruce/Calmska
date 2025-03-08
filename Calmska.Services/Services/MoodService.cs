using Calmska.Models.DTO;
using Calmska.Models.Models;
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

        public async Task<OperationResultT<PaginatedResult<MoodDTO?>>> GetAllAsync(int? pageNumber, int? pageSize)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<MoodDTO?>>(_httpClient, "/moods");
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<MoodDTO?>>>> SearchAllByArgumentAsync(MoodDTO moodCriteria, int? pageNumber, int? pageSize)
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
                endpoint = $"/moods/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<MoodDTO?>>>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<MoodDTO?>> GetByArgumentAsync(MoodDTO moodCriteria)
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
                endpoint = $"/moods/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<MoodDTO?>(_httpClient, endpoint);
        }

        public async Task<OperationResultT<bool>> AddAsync(MoodDTO newAccount)
        {
            return await HttpClientHelper.PostAsync<MoodDTO?>(_httpClient, "/moods", newAccount);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(MoodDTO updatedAccount)
        {
            return await HttpClientHelper.PutAsync<MoodDTO?>(_httpClient, "/moods", updatedAccount);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(Guid moodId)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"/moods?moodId={moodId}");
        }
    }
}

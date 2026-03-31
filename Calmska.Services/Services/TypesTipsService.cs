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
    public class TypesTipsService : ITypesService<Types_TipsDTO>
    {
        private readonly HttpClient _httpClient;
        public TypesTipsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OperationResultT<PaginatedResult<Types_TipsDTO?>>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken token)
        {
            return await HttpClientHelper.GetAsync<PaginatedResult<Types_TipsDTO?>>(_httpClient, "types_tips", token);
        }

        public async Task<OperationResultT<PaginatedResult<IEnumerable<Types_TipsDTO?>>>> SearchAllByArgumentAsync(Types_TipsDTO typesCriteria, int? pageNumber, int? pageSize, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (typesCriteria != null)
            {
                if (typesCriteria.TypeId.HasValue)
                {
                    endpointParameters.Add($"TypeId={typesCriteria.TypeId}");
                }
                if (!string.IsNullOrEmpty(typesCriteria.Type))
                {
                    endpointParameters.Add($"Type={Uri.EscapeDataString(typesCriteria.Type)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"types_tips/searchList?{queryString}";
            }

            return await HttpClientHelper.GetAsync<PaginatedResult<IEnumerable<Types_TipsDTO?>>>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<Types_TipsDTO?>> GetByArgumentAsync(Types_TipsDTO typesCriteria, CancellationToken token)
        {
            List<string> endpointParameters = new();
            string endpoint = string.Empty;

            if (typesCriteria != null)
            {
                if (typesCriteria.TypeId.HasValue)
                {
                    endpointParameters.Add($"TypeId={typesCriteria.TypeId}");
                }
                if (!string.IsNullOrEmpty(typesCriteria.Type))
                {
                    endpointParameters.Add($"Type={Uri.EscapeDataString(typesCriteria.Type)}");
                }
                var queryString = string.Join("&", endpointParameters);
                endpoint = $"types_tips/search?{queryString}";
            }

            return await HttpClientHelper.GetAsync<Types_TipsDTO?>(_httpClient, endpoint, token);
        }

        public async Task<OperationResultT<bool>> AddAsync(Types_TipsDTO newTip, CancellationToken token)
        {
            return await HttpClientHelper.PostAsync<Types_TipsDTO?>(_httpClient, "types_tips", newTip, token);
        }

        public async Task<OperationResultT<bool>> UpdateAsync(Types_TipsDTO updatedTip, CancellationToken token)
        {
            return await HttpClientHelper.PutAsync<Types_TipsDTO?>(_httpClient, "types_tips", updatedTip, token);
        }

        public async Task<OperationResultT<bool>> DeleteAsync(int TypeId, CancellationToken token)
        {
            return await HttpClientHelper.DeleteAsync(_httpClient, $"types_tips?TypeId={TypeId}", token);
        }
    }
}

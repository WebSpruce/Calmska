using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Calmska.Domain.Common;

namespace Calmska.Services.Helper
{
    internal class HttpClientHelper
    {
        internal static async Task<OperationResultT<T>> GetAsync<T>(HttpClient client, string endpoint, CancellationToken token)
        {
            endpoint = $"{client.BaseAddress?.AbsoluteUri}{endpoint}";
            var result = new OperationResultT<T>();
            try
            {
                var response = await client.GetAsync(endpoint, token);
                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Request failed with status code {response.StatusCode}. Endpoint: {endpoint}";
                    return result;
                }
                var json = await response.Content.ReadAsStringAsync(token);
                try
                {
                    result.Result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    result.Error = string.Empty;
                }
                catch (JsonException jsonEx)
                {
                    result.Error = $"Serialization error: {jsonEx.Message}. Endpoint: {endpoint}. Response: {json}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                result.Error = $"HTTP error: {httpEx.Message}. Endpoint: {endpoint}";
            }
            catch (Exception ex)
            {
                result.Error = $"Unexpected error: {ex.Message}. Endpoint: {endpoint}";
            }

            return result;
        }
        internal static async Task<OperationResultT<bool>> PostAsync<T>(HttpClient client, string endpoint, T data, CancellationToken token)
        {
            endpoint = $"{client.BaseAddress?.AbsoluteUri}{endpoint}";
            var result = new OperationResultT<bool>();
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content, token);

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Request failed with status code {response.StatusCode}. Endpoint: {endpoint}";
                    result.Result = false;
                    return result;
                }

                result.Error = string.Empty;
                result.Result = true;
            }
            catch (HttpRequestException httpEx)
            {
                result.Error = $"HTTP error: {httpEx.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }
            catch (Exception ex)
            {
                result.Error = $"Unexpected error: {ex.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }

            return result;
        }
    
        internal static async Task<OperationResultT<bool>> PutAsync<T>(HttpClient client, string endpoint, T data, CancellationToken token)
        {
            endpoint = $"{client.BaseAddress?.AbsoluteUri}{endpoint}";
            var result = new OperationResultT<bool>();
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(endpoint, content, token);

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Request failed with status code {response.StatusCode}. Endpoint: {endpoint}";
                    result.Result = false;
                    return result;
                }

                result.Error = string.Empty;
                result.Result = true;
            }
            catch (HttpRequestException httpEx)
            {
                result.Error = $"HTTP error: {httpEx.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }
            catch (Exception ex)
            {
                result.Error = $"Unexpected error: {ex.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }

            return result;
        }
        internal static async Task<OperationResultT<bool>> DeleteAsync(HttpClient client, string endpoint, CancellationToken token)
        {
            endpoint = $"{client.BaseAddress?.AbsoluteUri}{endpoint}";
            var result = new OperationResultT<bool>();
            try
            {
                var response = await client.DeleteAsync(endpoint, token);

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Request failed with status code {response.StatusCode}. Endpoint: {endpoint}";
                    result.Result = false;
                    return result;
                }

                result.Error = string.Empty;
                result.Result = true;
            }
            catch (HttpRequestException httpEx)
            {
                result.Error = $"HTTP error: {httpEx.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }
            catch (Exception ex)
            {
                result.Error = $"Unexpected error: {ex.Message}. Endpoint: {endpoint}";
                result.Result = false;
            }

            return result;
        }
    }
}

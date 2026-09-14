using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Calmska.ApiClients.Interfaces;
using Calmska.Domain.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calmska.ApiClients
{
    public sealed class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;
    
        private static readonly string[] SensitiveKeys =
        {
            "password", "passwordhash", "secret", "secretkey", "apikey", "api_key",
            "token", "accesstoken", "refreshtoken", "connectionstring", "connection",
            "authorization", "bearer", "privatekey", "certificate", "ssn",
            "socialsecurity", "creditcard", "cardnumber", "apisecret", "clientsecret"
        };
    
        private static readonly Regex SensitivePattern = new(
            $@"(?i)({string.Join("|", SensitiveKeys.Select(Regex.Escape))})\s*[:=]\s*[^\s,}}]+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
        private static readonly Regex StackTracePattern = new(
            @"\s+at\s+[\w\.]+\s*\(.*?\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
        public HttpClientService(
            HttpClient httpClient,
            ILogger<HttpClientService> logger,
            IOptions<JsonSerializerOptions> jsonOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = jsonOptions?.Value ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
    
            _baseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/')
                ?? throw new InvalidOperationException("HttpClient.BaseAddress is not configured.");
        }
    
        public async Task<OperationResultT<T>> GetAsync<T>(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            var fullUrl = BuildUrl(endpoint);
            var result = new OperationResultT<T>();
    
            try
            {
                _logger.LogDebug($"GET {fullUrl}");
                var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
                return await HandleGetResponseAsync(response, fullUrl, cancellationToken, result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return SetError(result, "Request was cancelled.", fullUrl);
            }
            catch (HttpRequestException httpEx)
            {
                return HandleHttpException(httpEx, fullUrl, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, fullUrl, result);
            }
        }
    
        public async Task<OperationResultT<bool>> PostAsync<T>(
            string endpoint,
            T data,
            CancellationToken cancellationToken = default)
        {
            var fullUrl = BuildUrl(endpoint);
            var result = new OperationResultT<bool>();
    
            try
            {
                _logger.LogDebug($"POST {fullUrl}");
                var response = await _httpClient.PostAsJsonAsync(fullUrl, data, _jsonOptions, cancellationToken);
                return await HandleMutationResponseAsync(response, fullUrl, cancellationToken, result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return SetError(result, "Request was cancelled.", fullUrl);
            }
            catch (HttpRequestException httpEx)
            {
                return HandleHttpException(httpEx, fullUrl, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, fullUrl, result);
            }
        }
    
        public async Task<OperationResultT<bool>> PutAsync<T>(
            string endpoint,
            T data,
            CancellationToken cancellationToken = default)
        {
            var fullUrl = BuildUrl(endpoint);
            var result = new OperationResultT<bool>();
    
            try
            {
                _logger.LogDebug($"PUT {fullUrl}");
                var response = await _httpClient.PutAsJsonAsync(fullUrl, data, _jsonOptions, cancellationToken);
                return await HandleMutationResponseAsync(response, fullUrl, cancellationToken, result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return SetError(result, "Request was cancelled.", fullUrl);
            }
            catch (HttpRequestException httpEx)
            {
                return HandleHttpException(httpEx, fullUrl, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, fullUrl, result);
            }
        }
    
        public async Task<OperationResultT<bool>> DeleteAsync(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            var fullUrl = BuildUrl(endpoint);
            var result = new OperationResultT<bool>();
    
            try
            {
                _logger.LogDebug($"DELETE {fullUrl}");
                var response = await _httpClient.DeleteAsync(fullUrl, cancellationToken);
                return await HandleMutationResponseAsync(response, fullUrl, cancellationToken, result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return SetError(result, "Request was cancelled.", fullUrl);
            }
            catch (HttpRequestException httpEx)
            {
                return HandleHttpException(httpEx, fullUrl, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, fullUrl, result);
            }
        }

    
        //GET responses deserializes body to T
        private async Task<OperationResultT<T>> HandleGetResponseAsync<T>(
            HttpResponseMessage response,
            string fullUrl,
            CancellationToken cancellationToken,
            OperationResultT<T> result)
        {
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    result.Result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                    result.Error = string.Empty;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, $"Deserialization failed for {fullUrl}. Response: {json}");
                    return SetError(result, SanitizeError($"Deserialization failed: {jsonEx.Message}", fullUrl), fullUrl);
                }
                return result;
            }
    
            return await HandleErrorResponseAsync(response, fullUrl, cancellationToken, result);
        }
    
        //POST/PUT/DELETE responses success is boolean true
        private async Task<OperationResultT<bool>> HandleMutationResponseAsync(
            HttpResponseMessage response,
            string fullUrl,
            CancellationToken cancellationToken,
            OperationResultT<bool> result)
        {
            if (response.IsSuccessStatusCode)
            {
                result.Result = true;
                result.Error = string.Empty;
                return result;
            }
    
            return await HandleErrorResponseAsync(response, fullUrl, cancellationToken, result);
        }
    
        //Shared error handling for all verbs
        private async Task<OperationResultT<T>> HandleErrorResponseAsync<T>(
            HttpResponseMessage response,
            string fullUrl,
            CancellationToken cancellationToken,
            OperationResultT<T> result)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var sanitizedError = SanitizeErrorResponse(errorContent, response.StatusCode, fullUrl);
    
            _logger.LogWarning($"Request failed: {(int)response.StatusCode} {fullUrl} — {sanitizedError}");
    
            return SetError(result, sanitizedError, fullUrl);
        }
    
        private OperationResultT<T> HandleHttpException<T>(
            HttpRequestException httpEx,
            string fullUrl,
            OperationResultT<T> result)
        {
            _logger.LogError(httpEx, "HTTP request failed for {Url}", fullUrl);
            var sanitized = SanitizeError(httpEx.Message, fullUrl);
            return SetError(result, sanitized, fullUrl);
        }
    
        private OperationResultT<T> HandleException<T>(
            Exception ex,
            string fullUrl,
            OperationResultT<T> result)
        {
            _logger.LogError(ex, "Unexpected error for {Url}", fullUrl);
            var sanitized = SanitizeError(ex.Message, fullUrl);
            return SetError(result, sanitized, fullUrl);
        }
    
        private static OperationResultT<T> SetError<T>(
            OperationResultT<T> result,
            string error,
            string fullUrl)
        {
            result.Error = error;
            result.Result = default;
            return result;
        }
    
        private string SanitizeErrorResponse(string errorContent, HttpStatusCode statusCode, string fullUrl)
        {
            // 1. Try extract user-facing message from known error envelopes
            var userMessage = TryExtractUserMessage(errorContent);
            if (!string.IsNullOrWhiteSpace(userMessage))
                return SanitizeError(userMessage, fullUrl);
    
            // 2. Fallback: generic message by status code
            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Unauthorized. Please log in again.",
                HttpStatusCode.Forbidden => "Access denied.",
                HttpStatusCode.NotFound => "The requested resource was not found.",
                HttpStatusCode.TooManyRequests => "Too many requests. Please try again later.",
                HttpStatusCode.InternalServerError => "A server error occurred. Please try again later.",
                HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout
                    => "The service is temporarily unavailable. Please try again later.",
                HttpStatusCode.BadRequest => "Invalid request.",
                _ => SanitizeError($"Request failed with status code {(int)statusCode}.", fullUrl)
            };
        }
    
        private static string? TryExtractUserMessage(string errorContent)
        {
            if (string.IsNullOrWhiteSpace(errorContent))
                return null;

            if (!errorContent.TrimStart().StartsWith('{'))
                return errorContent.Trim();
    
            try
            {
                using var doc = JsonDocument.Parse(errorContent);
                var root = doc.RootElement;
    
                // RFC 7807 ProblemDetails + common variants
                var keys = new[] { "detail", "title", "error", "message", "errorMessage", "errors" };
                foreach (var key in keys)
                {
                    if (root.TryGetProperty(key, out var prop))
                    {
                        var value = prop.ValueKind switch
                        {
                            JsonValueKind.Array => string.Join("; ", prop.EnumerateArray().Select(e => e.GetString())),
                            JsonValueKind.String => prop.GetString(),
                            _ => prop.ToString()
                        };
    
                        if (!string.IsNullOrWhiteSpace(value))
                            return value;
                    }
                }
            }
            catch { /* ignore — fall back to generic */ }
    
            return null;
        }
    
        private string SanitizeError(string rawMessage, string fullUrl)
        {
            if (string.IsNullOrWhiteSpace(rawMessage))
                return "An error occurred while processing the request.";
    
            var sanitized = rawMessage;
    
            // 1. Redact full URL and base URL
            sanitized = sanitized.Replace(fullUrl, "[REDACTED_URL]", StringComparison.OrdinalIgnoreCase);
            sanitized = sanitized.Replace(_baseUrl, "[BASE_URL]", StringComparison.OrdinalIgnoreCase);
    
            // 2. Redact sensitive key/value pairs
            sanitized = SensitivePattern.Replace(sanitized, "$1: [REDACTED]");
    
            // 3. Strip stack traces
            sanitized = StackTracePattern.Replace(sanitized, "");
    
            // 4. Truncate
            if (sanitized.Length > 300)
                sanitized = sanitized[..300] + "...";
    
            return string.IsNullOrWhiteSpace(sanitized)
                ? "An error occurred while processing the request."
                : sanitized;
        }
    
        private string BuildUrl(string endpoint)
        {
            var cleanEndpoint = endpoint.StartsWith('/') ? endpoint[1..] : endpoint;
            return $"{_baseUrl}/{cleanEndpoint}";
        }
    }
}

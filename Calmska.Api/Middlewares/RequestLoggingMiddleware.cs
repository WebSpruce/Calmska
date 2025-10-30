using System.Diagnostics;

namespace Calmska.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var request = context.Request;
        var requestInfo = $"{request.Method} {request.Path}";
        
        _logger.LogInformation($"STARTED '{requestInfo}'");
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation($"COMPLETED '{requestInfo}' in {elapsedTime}ms with status code: {statusCode}");
        }
    }
}
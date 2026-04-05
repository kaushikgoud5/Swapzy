using Swapzy.Core.Exceptions;
using System.Diagnostics;
using System.Text.Json;

namespace Swapzy.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            var (statusCode, errorCode, message) = exception switch
            {
                AppException appEx => (appEx.StatusCode, appEx.ErrorCode, appEx.Message),
                UnauthorizedAccessException => (401, "UNAUTHORIZED", "You are not authorized to perform this action."),
                InvalidOperationException => (400, "BAD_REQUEST", exception.Message),
                ArgumentException => (400, "VALIDATION_ERROR", exception.Message),
                _ => (500, "INTERNAL_ERROR", "An unexpected error occurred.")
            };

            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Unhandled exception | TraceId: {TraceId} | Path: {Path}",
                    traceId, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("Handled exception | TraceId: {TraceId} | Status: {Status} | Error: {Error} | Path: {Path}",
                    traceId, statusCode, exception.Message, context.Request.Path);
            }

            var response = new ApiErrorResponse
            {
                Status = statusCode,
                ErrorCode = errorCode,
                Message = _env.IsDevelopment() && statusCode >= 500 ? exception.Message : message,
                TraceId = traceId
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}

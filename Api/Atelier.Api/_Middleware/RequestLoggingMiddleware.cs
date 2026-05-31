using System.Diagnostics;
using System.Security.Claims;

namespace Atelier.Api._Middleware
{
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
            var requestId = Guid.NewGuid().ToString("N")[..8];

            var request = context.Request;

            _logger.LogInformation(
                "[{RequestId}] → {Method} {Path}{QueryString} | IP: {IP}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString,
                context.Connection.RemoteIpAddress
            );

            await _next(context);

            var user = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown"
                : "Anonymous";

            stopwatch.Stop();

            _logger.LogInformation(
                "[{RequestId}] ← {Method} {Path} | User: {User} | {StatusCode} | {ElapsedMs}ms",
                requestId,
                request.Method,
                request.Path,
                user,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}

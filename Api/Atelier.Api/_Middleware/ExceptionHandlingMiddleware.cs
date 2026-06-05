using Atelier.Api._DTOs;
using Atelier.Api._Exceptions;

namespace Atelier.Api._Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, response) = exception switch
            {
                StatsCalculatorException e  => (e.StatusCode,   (object)new ResponseType500Dto(e.Detail)),
                NoDataException e           => (e.StatusCode,   (object)new ResponseType404Dto(e.Detail)),
                ArgumentException e         => (400,            (object)new ResponseType400Dto(e.Message)),
                KeyNotFoundException e      => (404,            (object)new ResponseType404Dto(e.Message)),
                _                           => (500,            (object)new ResponseType500Dto("An unexpected error occurred"))
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

using Atelier.Api._DTOs;
using Atelier.Api._Exception;

namespace Atelier.Api._Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, response) = exception switch
            {
                NoDataException e       => (e.StatusCode,   (object) new ResponseType404Dto(e.Detail)),
                ArgumentException e     => (400,            (object)new ResponseType400Dto(e.Message)),
                KeyNotFoundException e  => (404,            (object)new ResponseType404Dto(e.Message)),
                _                       => (500,            (object)new ResponseType500Dto("An unexpected error occurred"))
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

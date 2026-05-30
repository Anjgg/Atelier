using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atelier.Api._Swagger
{
    public class SwaggerDocumentationFilter : IOperationFilter
    {
        private static readonly Dictionary<int, string> DefaultDescriptions = new()
        {
            { 200, "Request processed successfully" },
            { 201, "Resource created successfully" },
            { 400, "Invalid request parameters" },
            { 401, "Unauthenticated" },
            { 403, "Unauthorized" },
            { 404, "Resource not found" },
            { 500, "Internal server error" }
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attribute = context.MethodInfo
                .GetCustomAttributes(typeof(SwaggerDocumentationAttribute), false)
                .FirstOrDefault() as SwaggerDocumentationAttribute;

            if (attribute == null) return;

            operation.Summary = attribute.Summary;
            operation.Description = attribute.Description;

            foreach (var response in operation.Responses)
            {
                if (int.TryParse(response.Key, out int code) &&
                    DefaultDescriptions.TryGetValue(code, out string? description))
                {
                    response.Value.Description = description;
                }
            }
        }
    }
}

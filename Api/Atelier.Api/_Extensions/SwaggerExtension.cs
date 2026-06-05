using Atelier.Api._Swagger;
using Microsoft.OpenApi;

namespace Atelier.Api._Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDocumentationFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token"
                });
                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", doc)] = new List<string>()
                });
            });

            return services;
        }
    }
}

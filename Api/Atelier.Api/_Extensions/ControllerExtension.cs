using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api.Calculator;
using Atelier.Api.Helpers;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Atelier.Api._Extensions
{
    public static class ControllersExtensions
    {
        public static IServiceCollection AddControllersWithConfig(this IServiceCollection services)
        {
            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .ToDictionary(
                                e => e.Key,
                                e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                            );

                        return new BadRequestObjectResult(
                            new ResponseType400Dto(
                                detail: "One or more fields are invalid",
                                errors: errors));
                    };
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.UnmappedMemberHandling =
                        JsonUnmappedMemberHandling.Disallow;
                });

            return services;
        }
    }
}

using Atelier.Api._Entities;
using Atelier.Api.Calculator;
using Atelier.Api.Helpers;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace Atelier.Api._Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IPlayerHelper, PlayerHelper>();
            services.AddScoped<IStatsService, StatsService>();
            services.AddScoped<IStatsCalculator, StatsCalculator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}

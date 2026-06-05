using Atelier.Api._Data;
using Microsoft.EntityFrameworkCore;

namespace Atelier.Api._Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=Tennis.db"));
            return services;
        }

        public static WebApplication InitializeDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            Seeder.Seed(context);

            return app;
        }
    }
}

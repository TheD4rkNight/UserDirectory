using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserDirectory.Application.Common;
using UserDirectory.Application.Users;
using UserDirectory.Infrastructure.Repositories;

namespace UserDirectory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dbPath = Path.Combine(baseDir, "data", "app.db");
        var connectionString = $"Data Source={dbPath}";

        //var connectionString = configuration.GetConnectionString("DefaultConnection")
        //    ?? "Data Source=./data/app.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

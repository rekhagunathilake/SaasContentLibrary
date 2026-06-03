using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaasContentLibrary.Application.Common.Interfaces;
using SaasContentLibrary.Infrastructure.Persistence;
using SaasContentLibrary.Infrastructure.Persistence.Repositories;

namespace SaasContentLibrary.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ContentLibrary") ??
            throw new InvalidOperationException("Connection string 'ContentLibrary' not configured.");

        services.AddDbContext<SaasContentLibraryDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => 
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "content")));

        services.AddScoped<IContentBlockRepository, ContentBlockRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

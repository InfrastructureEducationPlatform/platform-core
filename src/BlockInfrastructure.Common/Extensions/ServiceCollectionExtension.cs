using BlockInfrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BlockInfrastructure.Common.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(option =>
        {
            option.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));
            option.EnableSensitiveDataLogging();
            option.EnableDetailedErrors();
            option.AddInterceptors(new InvalidateCacheInterceptor());
        });

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));
        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
using BlockInfrastructure.Files.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockInfrastructure.Files.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        services.AddSingleton<FileTypeVerifierService>();
        services.AddScoped<FileService>();
        return services;
    }
}
using BlockInfrastructure.Common.Configurations;
using BlockInfrastructure.Common.Services;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace BlockInfrastructure.Common.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration,
                                                       IWebHostEnvironment environment)
    {
        services.AddSingleton<IJwtService, JwtService>();
        services.Configure<AuthConfiguration>(configuration.GetSection("Auth"));
        services.AddDbContext<DatabaseContext>(option =>
        {
            option.UseNpgsql(configuration.GetConnectionString("DatabaseConnection"));
            option.EnableSensitiveDataLogging();
            option.EnableDetailedErrors();
            option.AddInterceptors(new InvalidateCacheInterceptor());
        });

        var redisConnection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection"));
        services.AddSingleton<IConnectionMultiplexer>(redisConnection);
        services.AddSingleton<ICacheService, CacheService>();

        // Monitoring Section
        services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(environment.ApplicationName);
                    resource.AddEnvironmentVariableDetector();
                })
                .WithTracing(tracing =>
                {
                    var excludeEndpointList = new List<string>
                    {
                        "/healthz",
                        "/metrics",
                        "/loki/api/v1/push"
                    };
                    tracing
                        .AddSource(DiagnosticHeaders.DefaultListenerName)
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddAspNetCoreInstrumentation(opt =>
                        {
                            opt.Filter = httpContext =>
                            {
                                var pathValue = httpContext.Request.Path.Value;
                                return !excludeEndpointList.Contains(pathValue);
                            };
                            opt.EnrichWithHttpRequest = (activity, request) =>
                            {
                                activity.DisplayName = request.Method + " " + request.Path;
                            };
                        })
                        .AddNpgsql()
                        .AddRedisInstrumentation(redisConnection)
                        .AddHttpClientInstrumentation(opt =>
                        {
                            opt.EnrichWithHttpRequestMessage = (activity, message) =>
                            {
                                activity.DisplayName = message.Method + " " + message.RequestUri;
                            };
                            opt.FilterHttpRequestMessage = message =>
                            {
                                var pathValue = message.RequestUri?.AbsolutePath;
                                return !excludeEndpointList.Contains(pathValue);
                            };
                        })
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                           .AddService("BlockInfrastructure-Core")
                                           .AddEnvironmentVariableDetector()
                                           .AddTelemetrySdk()
                        )
                        .AddOtlpExporter(option => option.Endpoint = new Uri(configuration["otlp"]));
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddMeter(InstrumentationOptions.MeterName)
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddOtlpExporter((option, metricReaderOptions) =>
                        {
                            option.Endpoint = new Uri(configuration["otlp"]);
                            metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 60 * 1000;
                        });
                });


        return services;
    }
}
using System.Reflection;
using BlockInfrastructure.BackgroundCacheWorker.Extensions;
using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Configurations;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Services.Authentication;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, service, configuration) =>
{
    configuration.ReadFrom.Configuration(ctx.Configuration)
                 .ReadFrom.Services(service)
                 .Enrich.FromLogContext()
                 .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
                 .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                 .WriteTo.Console()
                 .WriteTo.GrafanaLoki(builder.Configuration["loki"], propertiesAsLabels: new List<string>
                 {
                     "Application",
                     "Environment",
                     "RequestId"
                 });
});

// Add Controllers
builder.Services.AddControllers(options => options.Filters.Add(typeof(GlobalExceptionFilter)));

// Configure Configurations
builder.Services.Configure<ConnectionConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomOperationIds(apiDesc =>
        apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);

    options.AddSecurityDefinition("JwtAuthenticationFilter", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header
    });

    options.OperationFilter<SwaggerOperationFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Authentication
builder.Services.Configure<AuthConfiguration>(builder.Configuration.GetSection("Auth"));
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddTransient<GoogleAuthenticationService>();
builder.Services.AddTransient<SelfAuthenticationProvider>();
builder.Services.AddSingleton<AuthProviderServiceFactory>(serviceProvider => provider =>
{
    return provider switch
    {
        CredentialProvider.Google => serviceProvider.GetRequiredService<GoogleAuthenticationService>(),
        CredentialProvider.Self => serviceProvider.GetRequiredService<SelfAuthenticationProvider>(),
        _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown Provider")
    };
});
builder.Services.AddHttpClient(HttpClientNames.GoogleApi, client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com");
});
builder.Services.AddHttpClient(HttpClientNames.GoogleOAuthApi, client =>
{
    client.BaseAddress = new Uri("https://oauth2.googleapis.com");
});
builder.Services.AddHttpClient(HttpClientNames.DeploymentApi, (provider, client) =>
{
    var connectionConfiguration = provider.GetRequiredService<IOptionsMonitor<ConnectionConfiguration>>().CurrentValue;
    client.BaseAddress = new Uri(connectionConfiguration.DeploymentPluginConnection);
});

// Add Channel
builder.Services.AddScoped<ChannelService>();

// Add User
builder.Services.AddScoped<IUserService, UserService>();

// Add Sketches
builder.Services.AddScoped<SketchService>();

// Add Deployment
builder.Services.AddScoped<IDeploymentService, DeploymentService>();

// Add Common
builder.Services.AddCommonServices(builder.Configuration, builder.Environment);

// Add Shared Configurations
builder.Services.AddCors();
builder.Services.AddMassTransit(configurator =>
{
    configurator.RegisterBackgroundCacheConsumers();
    configurator.UsingRabbitMq((ctx, busFactoryConfigurator) =>
    {
        // MassTransit Default
        busFactoryConfigurator.Host(builder.Configuration["RabbitMq:Host"],
            Convert.ToUInt16(builder.Configuration["RabbitMq:Port"]), "/", h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"]);
                h.Password(builder.Configuration["RabbitMq:Password"]);
            });

        // Configure Message
        busFactoryConfigurator.Message<UserStateModifiedEvent>(a => a.SetEntityName("user.modified"));

        // Configure Cache Consumer(Worker)
        busFactoryConfigurator.ConfigureBackgroundCacheEndpoint(ctx);
    });
});

// Add Background Cache Worker
builder.Services.AddBackgroundCacheWorker();

// Add HealthCheck
builder.Services.AddHealthChecks()
       .AddDbContextCheck<DatabaseContext>();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Migrate Database
using (var scope = app.Services.CreateScope())
{
    var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await databaseContext.Database.MigrateAsync();
}

app.UseCors(a => a.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
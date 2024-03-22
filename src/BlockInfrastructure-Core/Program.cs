using System.Reflection;
using BlockInfrastructure.BackgroundCacheWorker.Extensions;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Configurations;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Services.Authentication;
using BlockInfrastructure.Core.Services.Consumers;
using BlockInfrastructure.Files.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
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
                 .WriteTo.OpenTelemetry(ctx.Configuration["otlp"], resourceAttributes: new Dictionary<string, object>
                 {
                     ["service.name"] = ctx.HostingEnvironment.ApplicationName,
                     ["deployment.environment"] = ctx.HostingEnvironment.EnvironmentName
                 });
});

// Add Controllers
builder.Services.AddControllers(options => options.Filters.Add(typeof(GlobalExceptionFilter)))
       .ConfigureApplicationPartManager(manager => manager.FeatureProviders.Add(new InternalControllerFeatureProvider()));

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

    // Include Swagger XML Documentation.
    var includedList = new List<string>
    {
        "BlockInfrastructure-Core.xml",
        "BlockInfrastructure.Files.xml"
    };

    foreach (var eachList in includedList)
    {
        var path = Path.Combine(AppContext.BaseDirectory, eachList);
        if (File.Exists(path))
        {
            options.IncludeXmlComments(path);
        }
    }
});

// Add Authentication
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
builder.Services.AddScoped<IChannelService, ChannelService>();

// Add User
builder.Services.AddScoped<IUserService, UserService>();

// Add Sketches
builder.Services.AddScoped<SketchService>();

// Add Deployment
builder.Services.AddScoped<IDeploymentService, DeploymentService>();

// Add Plugin Service
builder.Services.AddScoped<PluginService>();

// Add Pricing Service
builder.Services.AddScoped<PricingService>();

// Add Common
builder.Services.AddCommonServices(builder.Configuration, builder.Environment);

// Add Shared Configurations
builder.Services.AddCors();
builder.Services.AddMassTransit(configurator =>
{
    configurator.RegisterBackgroundCacheConsumers();
    configurator.AddConsumer<DeploymentAcceptedEventConsumer>();
    configurator.AddConsumer<DeploymentResultEventConsumer>();

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
        busFactoryConfigurator.Message<StartDeploymentEvent>(a => a.SetEntityName("deployment.started"));

        // Configure Cache Consumer(Worker)
        busFactoryConfigurator.ConfigureBackgroundCacheEndpoint(ctx);

        busFactoryConfigurator.ReceiveEndpoint("deployment.accepted.core-update-status", cfg =>
        {
            // Setup Consumer
            cfg.Bind("deployment.accepted");
            cfg.ConfigureConsumer<DeploymentAcceptedEventConsumer>(ctx);
        });

        busFactoryConfigurator.ReceiveEndpoint("deployment.result.core-update-status", cfg =>
        {
            // Setup Consumer
            cfg.Bind("deployment.result");
            cfg.ConfigureConsumer<DeploymentResultEventConsumer>(ctx);
        });
    });
});
builder.Services.AddMediatR(mediatRConfiguration =>
{
    // Add Core
    mediatRConfiguration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

    // Add Background Cache
    mediatRConfiguration.ConfigureMediatRBackgroundCache();
});

// Add Background Cache Worker
builder.Services.AddBackgroundCacheWorker();

// Add File Module
builder.Services.AddFileServices();

// Add HealthCheck
builder.Services.AddHealthChecks()
       .AddDbContextCheck<DatabaseContext>();

var app = builder.Build();

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
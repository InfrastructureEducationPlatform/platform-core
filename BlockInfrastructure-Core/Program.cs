using System.Reflection;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Configurations;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<AuthProviderServiceFactory>(serviceProvider => provider =>
{
    return provider switch
    {
        CredentialProvider.Google => serviceProvider.GetRequiredService<GoogleAuthenticationService>(),
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

// Add Channel
builder.Services.AddScoped<ChannelService>();

// Add User
builder.Services.AddScoped<UserService>();

// Add Shared Configurations
builder.Services.AddCors();
builder.Services.AddDbContext<DatabaseContext>((services, option) =>
{
    var connectionConfiguration = services.GetRequiredService<IOptionsSnapshot<ConnectionConfiguration>>().Value;
    option.UseNpgsql(connectionConfiguration.DatabaseConnection);
    option.EnableSensitiveDataLogging();
    option.EnableDetailedErrors();
});

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

app.UseMiddleware<CustomAuthenticationMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
using System.Reflection;
using BlockInfrastructure.Core.Configurations;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Configure Configurations
builder.Services.Configure<ConnectionConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomOperationIds(apiDesc =>
        apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Shared Configurations
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

app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
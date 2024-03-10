using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Services.Consumers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlockInfrastructure.Core.Test.Service.Consumers;

public class DeploymentResultEventConsumerTest : IDisposable
{
    private readonly UnitTestDatabaseContext _databaseContext;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestHarness _testHarness;

    public DeploymentResultEventConsumerTest()
    {
        _mockCacheService = new Mock<ICacheService>();
        _databaseContext = new UnitTestDatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
                                                       .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);
        _mockCacheService = new Mock<ICacheService>();

        _serviceProvider = new ServiceCollection()
                           .AddSingleton(_mockCacheService.Object)
                           .AddScoped<DatabaseContext>(_ => _databaseContext)
                           .AddMassTransitTestHarness(x =>
                           {
                               x.AddConsumer<DeploymentResultEventConsumer>();
                               x.UsingInMemory((context, cfg) =>
                               {
                                   cfg.ReceiveEndpoint("deployment.accepted.core-update-status", cfg =>
                                   {
                                       // Setup Consumer
                                       cfg.Bind("deployment.result");
                                       cfg.ConfigureConsumer<DeploymentResultEventConsumer>(context);
                                   });
                               });
                           })
                           .BuildServiceProvider();
        _testHarness = _serviceProvider.GetRequiredService<ITestHarness>();
        _testHarness.Start().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _serviceProvider.DisposeAsync().GetAwaiter().GetResult();
    }

    [Fact(DisplayName = "Consume: Consume은 만약 이벤트가 성공 이벤트이면 DeploymentStatus를 Deployed로 변경합니다.")]
    public async Task Is_Consume_Update_Deployment_Status_To_Deployed_When_Event_Is_Success()
    {
        // Let
        var deployment = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = "asdf",
            CapturedBlockData = JsonDocument.Parse("{}"),
            PluginInstallationId = Ulid.NewUlid().ToString(),
            SketchId = Ulid.NewUlid().ToString()
        };
        _databaseContext.DeploymentLogs.Add(deployment);
        await _databaseContext.SaveChangesAsync();
        _databaseContext.Entry(deployment).State = EntityState.Detached;

        // Do
        await _testHarness.Bus.Publish(new DeploymentResultEvent
        {
            DeploymentId = deployment.Id,
            IsSuccess = true,
            DeploymentOutputList = JsonDocument.Parse("{}")
        });

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<DeploymentResultEventConsumer>();
        Assert.True(await consumed.Consumed.Any<DeploymentResultEvent>());
    }

    [Fact(DisplayName = "Consume: Consume은 만약 이벤트가 실패 이벤트이면 DeploymentStatus를 Failed로 변경합니다.")]
    public async Task Is_Consume_Update_Deployment_Status_To_Failed_When_Event_Is_Failed()
    {
        // Let
        var deployment = new DeploymentLog
        {
            Id = Ulid.NewUlid().ToString(),
            DeploymentStatus = DeploymentStatus.Created,
            ChannelId = "asdf",
            CapturedBlockData = JsonDocument.Parse("{}"),
            PluginInstallationId = Ulid.NewUlid().ToString(),
            SketchId = Ulid.NewUlid().ToString()
        };
        _databaseContext.DeploymentLogs.Add(deployment);
        await _databaseContext.SaveChangesAsync();
        _databaseContext.Entry(deployment).State = EntityState.Detached;

        // Do
        await _testHarness.Bus.Publish(new DeploymentResultEvent
        {
            DeploymentId = deployment.Id,
            IsSuccess = false,
            DeploymentOutputList = JsonDocument.Parse("{}")
        });

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<DeploymentResultEventConsumer>();
        Assert.True(await consumed.Consumed.Any<DeploymentResultEvent>());
    }
}
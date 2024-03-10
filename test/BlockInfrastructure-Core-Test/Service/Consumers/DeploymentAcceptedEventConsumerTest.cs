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

public class DeploymentAcceptedEventConsumerTest : IDisposable
{
    private readonly UnitTestDatabaseContext _databaseContext;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestHarness _testHarness;

    public DeploymentAcceptedEventConsumerTest()
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
                               x.AddConsumer<DeploymentAcceptedEventConsumer>();
                               x.UsingInMemory((context, cfg) =>
                               {
                                   cfg.ReceiveEndpoint("deployment.accepted.core-update-status", cfg =>
                                   {
                                       // Setup Consumer
                                       cfg.Bind("deployment.accepted");
                                       cfg.ConfigureConsumer<DeploymentAcceptedEventConsumer>(context);
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

    [Fact(DisplayName = "Consume: Consume은 만약 이벤트 타입이 DeploymentAccepted이라면 특정 배포에 대한 상태를 업데이트합니다.")]
    public async Task Is_Consume_Update_Deployment_Status_Directly_When_Type_Is_DeploymentAccepted()
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
        await _testHarness.Bus.Publish(new DeploymentAcceptedEvent
        {
            DeploymentId = deployment.Id
        });

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<DeploymentAcceptedEventConsumer>();
        Assert.True(await consumed.Consumed.Any<DeploymentAcceptedEvent>());
    }
}
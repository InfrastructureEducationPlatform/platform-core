using BlockInfrastructure.BackgroundCacheWorker.Consumers.Channels;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Messages;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BlockInfrastructure.BackgroundCacheWorker.Test.Consumers.Channels;

public class ResetChannelInformationCacheConsumerTest : IDisposable
{
    private readonly UnitTestDatabaseContext _databaseContext;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestHarness _testHarness;

    public ResetChannelInformationCacheConsumerTest()
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
                               x.AddConsumer<ResetChannelInformationCacheConsumer>();
                               x.UsingInMemory((context, cfg) =>
                               {
                                   cfg.ReceiveEndpoint("channel.modified.invalidate-channel-information", cfg =>
                                   {
                                       // Setup Consumer
                                       cfg.Bind("channel.modified");
                                       cfg.ConfigureConsumer<ResetChannelInformationCacheConsumer>(context);
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

    [Fact(DisplayName = "Consume: Consume should invalidate its cache well.")]
    public async Task Is_Consume_Invalidates_Cache_Well()
    {
        // Let
        var channel = new Channel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Name",
            Description = "testDesc",
            ProfileImageUrl = "testUrl",
            ChannelPermissionList = new List<ChannelPermission>()
        };
        _databaseContext.Channels.Add(channel);
        await _databaseContext.SaveChangesAsync();

        // Do
        await _testHarness.Bus.Publish(new ChannelStateModifiedEvent
        {
            ChannelId = channel.Id
        });

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<ResetChannelInformationCacheConsumer>();
        Assert.True(await consumed.Consumed.Any<ChannelStateModifiedEvent>());

        // Check Cache Called
        _mockCacheService.Verify(a => a.DeleteAsync(CacheKeys.ChannelInformationKey(channel.Id)), Times.Once);
    }
}
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

    [Fact(DisplayName = "Consume: Consume은 만약 이벤트 타입이 ForChannel이라면 특정 채널에 대한 캐시를 삭제합니다.")]
    public async Task Is_Consume_Invalidate_Channel_Cache_Directly_When_Type_Is_ForChannel()
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
        await _testHarness.Bus.Publish(ChannelStateModifiedEvent.ForChannel(channel.Id));

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<ResetChannelInformationCacheConsumer>();
        Assert.True(await consumed.Consumed.Any<ChannelStateModifiedEvent>());

        // Check Cache Called
        _mockCacheService.Verify(a => a.DeleteAsync(CacheKeys.ChannelInformationKey(channel.Id)), Times.Once);
    }

    [Fact(DisplayName = "Consume: Consume은 만약 이벤트 타입이 ForUser이라면 특정 사용자가 소속되어 있는 채널에 대해 모든 캐시를 삭제합니다.")]
    public async Task Is_Consume_Invalidate_Channel_Cache_For_User()
    {
        // Let
        var user = new Common.Models.Data.User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "test@test.com",
            ProfilePictureImageUrl = null,
            ChannelPermissionList = new List<ChannelPermission>
            {
                new()
                {
                    Channel = new Channel
                    {
                        Id = Ulid.NewUlid().ToString(),
                        Name = "Test 1 Channel",
                        Description = "",
                        ProfileImageUrl = null
                    }
                },
                new()
                {
                    Channel = new Channel
                    {
                        Id = Ulid.NewUlid().ToString(),
                        Name = "Test 2 Channel",
                        Description = "",
                        ProfileImageUrl = null
                    }
                }
            }
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();

        // Do
        await _testHarness.Bus.Publish(ChannelStateModifiedEvent.ForUser(user.Id));

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<ResetChannelInformationCacheConsumer>();
        Assert.True(await consumed.Consumed.Any<ChannelStateModifiedEvent>());

        // Check Cache Called
        _mockCacheService.Verify(a => a.DeleteAsync(It.IsAny<string>()), Times.AtLeastOnce);
    }
}
using BlockInfrastructure.BackgroundCacheWorker.Consumers.User;
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

namespace BlockInfrastructure.BackgroundCacheWorker.Test.Consumers.User;

public class ResetMeProjectionCacheConsumerTest : IDisposable
{
    private readonly UnitTestDatabaseContext _databaseContext;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestHarness _testHarness;

    public ResetMeProjectionCacheConsumerTest()
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
                               x.AddConsumer<ResetMeProjectionCacheConsumer>();
                               x.UsingInMemory((context, cfg) =>
                               {
                                   cfg.ReceiveEndpoint("user.modified.invalidate-me", reCfg =>
                                   {
                                       reCfg.Bind("user.modified");
                                       reCfg.ConfigureConsumer<ResetMeProjectionCacheConsumer>(context);
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
        var user = new Common.Models.Data.User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "KangDroid",
            Email = "Test",
            ProfilePictureImageUrl = null,
            ChannelPermissionList = new List<ChannelPermission>
            {
                new()
                {
                    ChannelId = "test",
                    Channel = new Channel
                    {
                        Name = "Test",
                        Id = "test",
                        Description = ""
                    }
                }
            }
        };
        _databaseContext.Users.Add(user);
        await _databaseContext.SaveChangesAsync();

        // Do
        await _testHarness.Bus.Publish(new UserStateModifiedEvent
        {
            UserId = user.Id
        });

        // Check Message Consumed
        var consumed = _testHarness.GetConsumerHarness<ResetMeProjectionCacheConsumer>();
        Assert.True(await consumed.Consumed.Any<UserStateModifiedEvent>());

        // Check Cache Called
        _mockCacheService.Verify(a => a.DeleteAsync(CacheKeys.UserMeProjectionKey(user.Id)), Times.Once);
    }
}
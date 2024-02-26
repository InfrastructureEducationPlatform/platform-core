using System.Text.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Service;

public class PluginServiceTest
{
    private readonly UnitTestDatabaseContext _databaseContext =
        new(new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

    private readonly PluginService _pluginService;

    public PluginServiceTest()
    {
        _pluginService = new PluginService(_databaseContext);
    }

    [Fact(DisplayName = "ListAvailablePluginAsync: ListAvailablePluginAsync는 만약 플러그인이 없는 경우 빈 리스트를 반환합니다.")]
    public async Task Is_ListAvailablePluginAsync_Returns_Empty_List_When_No_Plugin_Exists()
    {
        // Do
        var pluginList = await _pluginService.ListAvailablePluginAsync(Ulid.NewUlid().ToString());

        // Check
        Assert.Empty(pluginList);
    }

    [Fact(DisplayName = "ListAvailablePluginAsync: ListAvailablePluginAsync는 플러그인이 있는 경우 플러그인 리스트를 반환합니다.")]
    public async Task Is_ListAvailablePluginAsync_Returns_Plugin_List_When_Plugin_Exists()
    {
        // Let
        var plugin = new Plugin
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Plugin",
            Description = "Test Plugin Description",
            SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        _databaseContext.Plugins.Add(plugin);
        await _databaseContext.SaveChangesAsync();

        // Do
        var pluginList = await _pluginService.ListAvailablePluginAsync(Ulid.NewUlid().ToString());

        // Check
        Assert.Single(pluginList);
        Assert.Equal(plugin.Id, pluginList.First().Id);
        Assert.Equal(plugin.Name, pluginList.First().Name);
        Assert.Equal(plugin.Description, pluginList.First().Description);
    }

    [Fact(DisplayName = "InstallPluginToChannelAsync: InstallPluginToChannelAsync는 플러그인을 채널에 설치합니다.")]
    public async Task Is_InstallPluginToChannelAsync_Installs_Plugin_To_Channel()
    {
        // Let
        var channelId = Ulid.NewUlid().ToString();
        var plugin = new Plugin
        {
            Id = Ulid.NewUlid().ToString(),
            Name = "Test Plugin",
            Description = "Test Plugin Description",
            SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new
            {
            })
        };
        _databaseContext.Plugins.Add(plugin);
        await _databaseContext.SaveChangesAsync();

        var installPluginRequest = new InstallPluginRequest
        {
            PluginId = plugin.Id,
            PluginConfiguration = JsonSerializer.SerializeToDocument(new
            {
            })
        };

        // Do
        await _pluginService.InstallPluginToChannelAsync(channelId, installPluginRequest);

        // Check
        var pluginInstallation = await _databaseContext.PluginInstallations.FirstOrDefaultAsync();
        Assert.NotNull(pluginInstallation);
        Assert.Equal(plugin.Id, pluginInstallation.PluginId);
        Assert.Equal(channelId, pluginInstallation.ChannelId);
        Assert.Equal(installPluginRequest.PluginConfiguration, pluginInstallation.PluginConfiguration);
    }
}
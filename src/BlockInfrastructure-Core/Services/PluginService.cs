using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class PluginService(DatabaseContext databaseContext)
{
    public async Task<List<PluginProjection>> ListAvailablePluginAsync()
    {
        var pluginList = await databaseContext.Plugins.ToListAsync();

        return pluginList.Select(PluginProjection.FromPlugin).ToList();
    }

    public async Task InstallPluginToChannelAsync(string channelId, InstallPluginRequest installPluginRequest)
    {
        var pluginInstallation = new PluginInstallation
        {
            PluginId = installPluginRequest.PluginId,
            ChannelId = channelId,
            PluginConfiguration = installPluginRequest.PluginConfiguration
        };
        databaseContext.PluginInstallations.Add(pluginInstallation);
        await databaseContext.SaveChangesAsync();
    }
}
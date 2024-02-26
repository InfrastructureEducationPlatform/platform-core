using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class PluginService(DatabaseContext databaseContext)
{
    public async Task<List<PluginProjection>> ListAvailablePluginAsync(string channelId)
    {
        var pluginList = await databaseContext.Plugins
                                              .Include(a => a.PluginInstallations)
                                              .ToListAsync();

        return pluginList.Select(a => PluginProjection.FromPlugin(a, channelId)).ToList();
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
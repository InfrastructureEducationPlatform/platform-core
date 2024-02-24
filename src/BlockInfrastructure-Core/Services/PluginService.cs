using BlockInfrastructure.Common.Services;
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
}
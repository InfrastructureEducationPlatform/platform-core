using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class PluginProjection
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public List<PluginTypeDefinition> PluginTypeDefinitions { get; set; } = new();

    public PluginInstallationProjection? PluginInstallation { get; set; }

    public static PluginProjection FromPlugin(Plugin plugin, string channelId)
    {
        return new PluginProjection
        {
            Id = plugin.Id,
            Name = plugin.Name,
            Description = plugin.Description,
            PluginTypeDefinitions = plugin.PluginTypeDefinitions,
            PluginInstallation =
                PluginInstallationProjection.FromPluginInstallation(
                    plugin.PluginInstallations.FirstOrDefault(a => a.ChannelId == channelId))
        };
    }
}

public class PluginInstallationProjection
{
    public string ChannelId { get; set; }
    public DateTimeOffset InstalledAt { get; set; }

    public static PluginInstallationProjection? FromPluginInstallation(PluginInstallation? pluginInstallation)
    {
        if (pluginInstallation is null)
        {
            return null;
        }

        return new PluginInstallationProjection
        {
            ChannelId = pluginInstallation.ChannelId,
            InstalledAt = pluginInstallation.CreatedAt
        };
    }
}
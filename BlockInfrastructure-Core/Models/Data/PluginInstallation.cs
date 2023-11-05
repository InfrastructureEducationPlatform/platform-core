using System.Text.Json;

namespace BlockInfrastructure_Core.Models.Data;

public class PluginInstallation : AutomaticAuditSupport
{
    public string ChannelId { get; set; }
    public Channel Channel { get; set; }

    public string PluginId { get; set; }
    public Plugin Plugin { get; set; }

    public JsonDocument PluginConfiguration { get; set; }
}
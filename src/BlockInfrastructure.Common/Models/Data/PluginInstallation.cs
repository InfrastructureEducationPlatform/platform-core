using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Common.Models.Data;

public class PluginInstallation : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    public string ChannelId { get; set; }
    public Channel Channel { get; set; }

    public string PluginId { get; set; }
    public Plugin Plugin { get; set; }

    public JsonDocument PluginConfiguration { get; set; }
}
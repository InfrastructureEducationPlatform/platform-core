using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure_Core.Models.Data;

public class Plugin : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public JsonDocument SamplePluginConfiguration { get; set; }

    public List<Channel> ChannelList { get; set; }
}
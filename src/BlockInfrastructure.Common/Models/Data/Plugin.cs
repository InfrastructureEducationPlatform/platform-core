using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BlockInfrastructure.Common.Models.Data;

public class Plugin : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public JsonDocument SamplePluginConfiguration { get; set; }

    public List<PluginTypeDefinition> PluginTypeDefinitions { get; set; } = new();

    public List<Channel> ChannelList { get; set; }
}

public class PluginTypeDefinition
{
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public string FieldDescription { get; set; }
    public bool IsRequired { get; set; }
    public string DefaultValue { get; set; }
}
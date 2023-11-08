namespace BlockInfrastructure.Core.Models.Data;

public class Channel : AutomaticAuditSupport
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<Sketch> SketchList { get; set; }
    public List<PluginInstallation> PluginInstallationList { get; set; }
    public List<ChannelPermission> ChannelPermissionList { get; set; }
}
namespace BlockInfrastructure.Common.Models.Data;

public class ChannelPermission : AutomaticAuditSupport
{
    public string UserId { get; set; }
    public User User { get; set; }

    public string ChannelId { get; set; }
    public Channel Channel { get; set; }

    public ChannelPermissionType ChannelPermissionType { get; set; }
}
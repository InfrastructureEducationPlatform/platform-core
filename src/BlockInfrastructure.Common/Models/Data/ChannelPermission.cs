using BlockInfrastructure.Common.Models.Messages;

namespace BlockInfrastructure.Common.Models.Data;

public class ChannelPermission : AutomaticAuditSupport, ICacheEventMessageGenerator
{
    public string UserId { get; set; }
    public User User { get; set; }

    public string ChannelId { get; set; }
    public Channel Channel { get; set; }

    public ChannelPermissionType ChannelPermissionType { get; set; }

    public List<object> GetCacheEventMessage()
    {
        return
        [
            new UserStateModifiedEvent
            {
                UserId = UserId
            },
            new ChannelStateModifiedEvent
            {
                ChannelId = ChannelId
            }
        ];
    }
}
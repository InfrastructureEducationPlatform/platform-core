using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Messages;

namespace BlockInfrastructure.Common.Models.Data;

public class User : AutomaticAuditSupport, ICacheEventMessageGenerator
{
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string? ProfilePictureImageUrl { get; set; }

    public List<Credential> CredentialList { get; set; }

    public List<ChannelPermission> ChannelPermissionList { get; set; }

    public List<object> GetCacheEventMessage()
    {
        return
        [
            new UserStateModifiedEvent
            {
                UserId = Id
            },
            ChannelStateModifiedEvent.ForUser(Id)
        ];
    }
}
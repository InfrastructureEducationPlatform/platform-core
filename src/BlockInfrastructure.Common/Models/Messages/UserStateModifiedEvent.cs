using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("user.modified")]
public class UserStateModifiedEvent
{
    public string UserId { get; set; }
}
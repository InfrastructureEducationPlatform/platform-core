using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("channel.modified")]
public class ChannelStateModifiedEvent
{
    public string ChannelId { get; set; }
}
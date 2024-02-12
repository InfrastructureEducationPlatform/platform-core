using MassTransit;

namespace BlockInfrastructure.Common.Models.Messages;

[EntityName("channel.modified")]
public class ChannelStateModifiedEvent
{
    public ChannelStateModifiedEventType ChannelStateModifiedEventType { get; set; }
    public Dictionary<string, string> EventMetadata { get; set; }

    public static ChannelStateModifiedEvent ForUser(string userId)
    {
        return new ChannelStateModifiedEvent
        {
            ChannelStateModifiedEventType = ChannelStateModifiedEventType.ForUser,
            EventMetadata = new Dictionary<string, string>
            {
                ["UserId"] = userId
            }
        };
    }

    public static ChannelStateModifiedEvent ForChannel(string channelId)
    {
        return new ChannelStateModifiedEvent
        {
            ChannelStateModifiedEventType = ChannelStateModifiedEventType.ForChannel,
            EventMetadata = new Dictionary<string, string>
            {
                ["ChannelId"] = channelId
            }
        };
    }
}

public enum ChannelStateModifiedEventType
{
    ForUser,
    ForChannel
}

public class ChannelStateModifiedForChannel
{
    public string ChannelId { get; set; }

    public static ChannelStateModifiedForChannel FromChannelStateModifiedEvent(ChannelStateModifiedEvent modifiedEvent)
    {
        if (modifiedEvent.ChannelStateModifiedEventType != ChannelStateModifiedEventType.ForChannel)
        {
            throw new InvalidOperationException(
                $"Tried to convert ChannelStateModifiedEvent to ChannelStateModifiedForChannel, but got {modifiedEvent.ChannelStateModifiedEventType}!");
        }

        return new ChannelStateModifiedForChannel
        {
            ChannelId = modifiedEvent.EventMetadata["ChannelId"]
        };
    }
}

public class ChannelStateModifiedForUser
{
    public string UserId { get; set; }

    public static ChannelStateModifiedForUser FromChannelStateModifiedEvent(ChannelStateModifiedEvent modifiedEvent)
    {
        if (modifiedEvent.ChannelStateModifiedEventType != ChannelStateModifiedEventType.ForUser)
        {
            throw new InvalidOperationException(
                $"Tried to convert ChannelStateModifiedEvent to ChannelStateModifiedForChannel, but got {modifiedEvent.ChannelStateModifiedEventType}!");
        }

        return new ChannelStateModifiedForUser
        {
            UserId = modifiedEvent.EventMetadata["UserId"]
        };
    }
}
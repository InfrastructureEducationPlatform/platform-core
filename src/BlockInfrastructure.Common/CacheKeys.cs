namespace BlockInfrastructure.Common;

public static class CacheKeys
{
    public static string UserMeProjectionKey(string userId)
    {
        return $"users/me-projection/{userId}";
    }

    public static string ChannelInformationKey(string channelId)
    {
        return $"channels/information/{channelId}";
    }
}
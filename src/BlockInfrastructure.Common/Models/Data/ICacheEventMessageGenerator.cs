namespace BlockInfrastructure.Common.Models.Data;

public interface ICacheEventMessageGenerator
{
    public List<object> GetCacheEventMessage();
}
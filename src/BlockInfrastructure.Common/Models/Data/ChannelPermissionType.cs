using System.Text.Json.Serialization;

namespace BlockInfrastructure.Common.Models.Data;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChannelPermissionType
{
    Owner,
    Reader
}
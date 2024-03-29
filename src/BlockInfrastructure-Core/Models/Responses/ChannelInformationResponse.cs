using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class ChannelInformationResponse
{
    [Required]
    public string ChannelId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public string? ProfileImageUrl { get; set; }

    [Required]
    public List<ChannelUserInformationProjection> ChannelUserInformationList { get; set; }

    public static ChannelInformationResponse FromChannelWithUser(Channel channel)
    {
        return new ChannelInformationResponse
        {
            ChannelId = channel.Id,
            Name = channel.Name,
            Description = channel.Description,
            ProfileImageUrl = channel.ProfileImageUrl,
            ChannelUserInformationList = channel.ChannelPermissionList
                                                .Select(a => ChannelUserInformationProjection.FromUser(a.User,
                                                    a.ChannelPermissionType)).ToList()
        };
    }
}

public class ChannelUserInformationProjection
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChannelPermissionType ChannelPermissionType { get; set; }

    public string? ProfilePictureImageUrl { get; set; }

    public static ChannelUserInformationProjection FromUser(User user, ChannelPermissionType permissionType)
    {
        return new ChannelUserInformationProjection
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            ProfilePictureImageUrl = user.ProfilePictureImageUrl,
            ChannelPermissionType = permissionType
        };
    }
}
using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Common.Models.Internal;

public class MeProjection
{
    /// <summary>
    ///     사용자 Unique ID
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    ///     사용자 이름(닉네임)
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    ///     사용자 이메일
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    ///     Optional, 사용자 프로필 이미지
    /// </summary>
    public string? ProfilePictureImageUrl { get; set; }

    /// <summary>
    ///     사용자가 속한 채널의 권한 리스트
    /// </summary>
    [Required]
    public List<ChannelPermissionProjection> ChannelPermissionList { get; set; } = new();

    public static MeProjection FromUser(User user)
    {
        return new MeProjection
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfilePictureImageUrl = user.ProfilePictureImageUrl,
            ChannelPermissionList = user.ChannelPermissionList
                                        .Select(a => new ChannelPermissionProjection
                                        {
                                            UserId = a.UserId,
                                            ChannelId = a.ChannelId,
                                            ChannelName = a.Channel.Name,
                                            ChannelPermissionType = a.ChannelPermissionType,
                                            CreatedAt = a.CreatedAt
                                        })
                                        .ToList()
        };
    }
}
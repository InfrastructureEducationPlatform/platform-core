using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Core.Models.Data;

namespace BlockInfrastructure.Core.Models.Responses;

public class ChannelPermissionProjection
{
    /// <summary>
    ///     사용자 ID
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    ///     사용자가 소속되어 있는 Channel ID
    /// </summary>
    [Required]
    public string ChannelId { get; set; }

    /// <summary>
    ///     채널 이름
    /// </summary>
    [Required]
    public string ChannelName { get; set; }

    /// <summary>
    ///     사용자가 소속되어 있는 채널의 권한
    /// </summary>
    [Required]
    public ChannelPermissionType ChannelPermissionType { get; set; }

    /// <summary>
    ///     권한이 추가된 시작
    /// </summary>
    [Required]
    public DateTimeOffset CreatedAt { get; set; }
}
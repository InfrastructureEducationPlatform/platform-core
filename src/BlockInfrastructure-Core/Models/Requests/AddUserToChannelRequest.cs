using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Requests;

public class AddUserToChannelRequest
{
    [Required]
    public string TargetUserId { get; set; }

    [Required]
    public ChannelPermissionType ChannelPermissionType { get; set; }
}
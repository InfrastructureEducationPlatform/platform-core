using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Core.Models.Requests;

public class UpdateUserChannelRoleRequest
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public ChannelPermissionType ChannelPermissionType { get; set; }
}
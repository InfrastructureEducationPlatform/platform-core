using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Core.Models.Requests;

public class UpdateChannelInformationRequest
{
    [Required]
    public string ChannelName { get; set; }

    [Required]
    public string ChannelDescription { get; set; }

    public string? ProfileImageUrl { get; set; }
}
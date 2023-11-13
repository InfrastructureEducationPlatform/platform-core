using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[JwtAuthenticationFilter]
[Route("/channels/{channelId}/sketches")]
public class SketchController(SketchService sketchService) : ControllerBase
{
    [HttpGet]
    [ChannelRole(ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
    public async Task<IActionResult> ListSketchesInChannelAsync(string channelId)
    {
        return Ok(await sketchService.ListSketches(channelId));
    }
}
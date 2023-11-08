using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/channels")]
public class ChannelController(ChannelService channelService) : ControllerBase
{
    [HttpPost]
    [JwtAuthenticationFilter]
    public async Task<IActionResult> CreateChannelAsync(CreateChannelRequest createChannelRequest)
    {
        var contextUser = HttpContext.GetUserContext();
        await channelService.CreateChannelAsync(createChannelRequest, contextUser);
        return Ok();
    }
}
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
    /// <summary>
    ///     채널을 생성하고, 생성한 사람을 Owner로 추가합니다.
    /// </summary>
    /// <param name="createChannelRequest">채널 생성 요청</param>
    /// <returns></returns>
    /// <response code="204">채널 생성에 성공한 경우</response>
    [HttpPost]
    [JwtAuthenticationFilter]
    public async Task<IActionResult> CreateChannelAsync(CreateChannelRequest createChannelRequest)
    {
        var contextUser = HttpContext.GetUserContext();
        await channelService.CreateChannelAsync(createChannelRequest, contextUser);
        return NoContent();
    }
}
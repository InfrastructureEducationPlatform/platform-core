using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/channels/{channelId}/plugins")]
public class PluginController(PluginService pluginService) : ControllerBase
{
    /// <summary>
    ///     서비스에서 허용하는 플러그인을 모두 리스팅 합니다.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">플러그인 목록 조회 성공 시</response>
    /// <response code="401">인증 실패 시</response>
    [HttpGet("available")]
    [JwtAuthenticationFilter]
    [ProducesResponseType<List<PluginProjection>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAvailablePlugins()
    {
        return Ok(await pluginService.ListAvailablePluginAsync());
    }
}
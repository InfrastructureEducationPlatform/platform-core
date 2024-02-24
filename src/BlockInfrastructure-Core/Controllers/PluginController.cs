using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Requests;
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

    [HttpGet("installed")]
    [JwtAuthenticationFilter]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    [ProducesResponseType<List<PluginProjection>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListInstalledPlugins(string channelId)
    {
        return Ok(await pluginService.ListInstalledPluginAsync(channelId));
    }

    /// <summary>
    ///     채널에 플러그인을 설치합니다.
    /// </summary>
    /// <param name="channelId">플러그인을 설치할 채널 Id</param>
    /// <param name="installPluginRequest">플러그인 설치 요청</param>
    /// <returns></returns>
    /// <response code="200">플러그인 설치 성공 시</response>
    /// <response code="401">인증 실패 시</response>
    /// <response code="403">채널에 대한 권한이 없는 경우</response>
    [HttpPost("install")]
    [JwtAuthenticationFilter]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> InstallPluginToChannelAsync(string channelId, InstallPluginRequest installPluginRequest)
    {
        await pluginService.InstallPluginToChannelAsync(channelId, installPluginRequest);
        return Ok();
    }
}
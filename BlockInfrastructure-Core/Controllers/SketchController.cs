using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[JwtAuthenticationFilter]
[Route("/channels/{channelId}/sketches")]
public class SketchController(SketchService sketchService) : ControllerBase
{
    /// <summary>
    ///     채널 내에 있는 모든 스케치를 가져옵니다,
    /// </summary>
    /// <remarks>
    ///     이 API는 채널의 Owner, Reader 모두 조회할 수 있습니다.
    /// </remarks>
    /// <param name="channelId">조회할 채널 ID</param>
    /// <returns></returns>
    /// <response code="200">조회에 성공한 경우.</response>
    /// <response code="401">인증 토큰이 없어 인증에 실패한 경우</response>
    /// <response code="403">스케치를 가져오는데 채널 권한이 부족한 경우(해당 API에서는 소속되어 있지 않은 채널을 조회하려 했을 때 반환)</response>
    [HttpGet]
    [ChannelRole(ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
    [ProducesResponseType<List<SketchResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListSketchesInChannelAsync(string channelId)
    {
        return Ok(await sketchService.ListSketches(channelId));
    }
}
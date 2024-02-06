using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
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

    /// <summary>
    ///     특정 채널의 정보와 채널의 사용자 정보를 가져옵니다.
    /// </summary>
    /// <param name="channelId">조회할 채널의 Id</param>
    /// <returns></returns>
    /// <response code="200">채널 정보 조회에 성공한 경우</response>
    /// <response code="401">인증 정보가 없는 경우</response>
    /// <response code="403">채널 조회 권한이 없는 경우</response>
    /// <response code="404">채널 정보가 존재하지 않는 경우</response>
    [JwtAuthenticationFilter]
    [HttpGet("{channelId}")]
    [ProducesResponseType<ChannelInformationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    public async Task<IActionResult> GetChannelInformationAsync(string channelId)
    {
        return Ok(await channelService.GetChannelInformationAsync(channelId));
    }

    /// <summary>
    ///     특정 채널의 기본 정보를 수정합니다.
    /// </summary>
    /// <param name="channelId">수정할 채널 Id</param>
    /// <param name="updateChannelInformationRequest">수정할 채널 정보</param>
    /// <returns></returns>
    /// <response code="204">채널 정보 수정에 성공한 경우</response>
    /// <response code="401">인증 정보가 없는 경우</response>
    /// <response code="403">채널 수정 권한이 없는 경우</response>
    /// <response code="404">채널 정보가 존재하지 않는 경우</response>
    [JwtAuthenticationFilter]
    [HttpPut("{channelId}")]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    public async Task<IActionResult> UpdateChannelInformationAsync(string channelId,
                                                                   UpdateChannelInformationRequest
                                                                       updateChannelInformationRequest)
    {
        await channelService.UpdateChannelInformationAsync(channelId, updateChannelInformationRequest);
        return NoContent();
    }
}
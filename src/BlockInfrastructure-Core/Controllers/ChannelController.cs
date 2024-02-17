using BlockInfrastructure.Common.Extensions;
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

    /// <summary>
    ///     특정 채널의 멤버의 권한을 수정합니다.
    /// </summary>
    /// <param name="channelId">채널 Id</param>
    /// <param name="updateUserChannelRoleRequest">사용자 ID와 채널 권한이 명시된 Request Body</param>
    /// <returns></returns>
    /// <response code="204">채널 정보 수정에 성공한 경우</response>
    /// <response code="400">만약 현재 요청하는 사람이 직접 수정하려고 하는 경우</response>
    /// <response code="401">인증 정보가 없는 경우</response>
    /// <response code="403">채널 수정 권한이 없는 경우</response>
    /// <response code="404">채널 정보가 존재하지 않는 경우</response>
    [JwtAuthenticationFilter]
    [HttpPut("{channelId}/users")]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    public async Task<IActionResult> UpdateUserChannelRoleAsync(string channelId,
                                                                UpdateUserChannelRoleRequest updateUserChannelRoleRequest)
    {
        var userContext = HttpContext.GetUserContext();
        await channelService.UpdateUserChannelRoleAsync(userContext.UserId, channelId, updateUserChannelRoleRequest);
        return NoContent();
    }

    /// <summary>
    ///     특정 채널에서 사용자(멤버)를 제거합니다.
    /// </summary>
    /// <param name="channelId">변경할 채널 Id</param>
    /// <param name="userId">사용자 Id</param>
    /// <returns></returns>
    /// <response code="204">채널 정보 수정에 성공한 경우</response>
    /// <response code="400">만약 현재 요청하는 사람이 본인을 삭제하려고 하는 경우</response>
    /// <response code="401">인증 정보가 없는 경우</response>
    /// <response code="403">채널 수정 권한이 없는 경우</response>
    /// <response code="404">채널 정보가 존재하지 않는 경우</response>
    [JwtAuthenticationFilter]
    [HttpDelete("{channelId}/users/{userId}")]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    public async Task<IActionResult> RemoveUserFromChannelAsync(string channelId, string userId)
    {
        var userContext = HttpContext.GetUserContext();
        await channelService.RemoveUserFromChannelAsync(userContext.UserId, channelId, userId);
        return NoContent();
    }

    /// <summary>
    ///     특정 채널에 사용자(멤버)를 추가합니다.
    /// </summary>
    /// <param name="channelId">채널 Id</param>
    /// <param name="addUserToChannelRequest">추가할 사용자의 Id와 권한을 명시한 Request Body</param>
    /// <returns></returns>
    /// <response code="204">채널 멤버 추가에 성공한 경우</response>
    /// <response code="400">만약 현재 요청하는 사람이 본인을 추가하려고 하는 경우</response>
    /// <response code="401">인증 정보가 없는 경우</response>
    /// <response code="403">채널 수정 권한이 없는 경우</response>
    /// <response code="409">만약 이미 채널에 사용자가 있는 경우</response>
    [JwtAuthenticationFilter]
    [HttpPost("{channelId}/users")]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    public async Task<IActionResult> AddUserToChannelAsync(string channelId, AddUserToChannelRequest addUserToChannelRequest)
    {
        var userContext = HttpContext.GetUserContext();
        await channelService.AddUserToChannelAsync(userContext.UserId, channelId, addUserToChannelRequest);
        return NoContent();
    }
}
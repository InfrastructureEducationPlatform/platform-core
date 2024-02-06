using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    ///     사용자의 현재 정보와, 소속되어 있는 채널 정보를 반환합니다.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">사용자 조회에 성공한 경우</response>
    /// <response code="404">사용자를 어떠한 이유로 찾을 수 없는 경우</response>
    [HttpGet("me")]
    [JwtAuthenticationFilter]
    [ProducesResponseType<MeProjection>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersDetailProjectionAsync()
    {
        var contextUser = HttpContext.GetUserContext();
        return Ok(await userService.GetMeAsync(contextUser));
    }

    /// <summary>
    ///     현재 사용자의 계정 설정을 업데이트 합니다. 업데이트 이후
    /// </summary>
    /// <remarks>MeProjection에 대한 Cache Invalidation을 진행하며, 클라이언트 측에서 ME API를 호출하여 최신 정보를 받아옵니다.</remarks>
    /// <param name="updateUserPreferenceRequest">Update Request</param>
    /// <returns></returns>
    /// <response code="204">사용자 정보 업데이트에 성공한 경우</response>
    /// <response code="401">사용자 인증에 실패한 경우</response>
    /// <response code="404">알 수 없는 에러로 사용자 정보를 찾을 수 없는 경우</response>
    [JwtAuthenticationFilter]
    [HttpPost("preferences")]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserPreferenceAsync(UpdateUserPreferenceRequest updateUserPreferenceRequest)
    {
        await userService.UpdatePreferenceAsync(HttpContext.GetUserContext(), updateUserPreferenceRequest);
        return NoContent();
    }
}
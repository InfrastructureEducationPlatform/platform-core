using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Responses;
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
}
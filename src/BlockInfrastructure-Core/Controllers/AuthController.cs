using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(AuthenticationService authenticationService) : ControllerBase
{
    /// <summary>
    ///     서비스에 로그인을 실시합니다.
    /// </summary>
    /// <remarks>
    ///     등록되지 않은 유저인 경우 NeedsRegistration과 JoinToken을 응답으로 반환합니다.
    /// </remarks>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    /// <response code="200">
    ///     OAuth에서 정보를 성공적으로 가져온 경우. 등록되지 않은 사용자는 LoginResult가 NeedsRegistration로 표기되고, 등록된 사용자는 로그인 성공으로 판단해
    ///     LoginSucceed를 반환합니다.
    /// </response>
    /// <response code="400">OAuth 로그인 혹은 OAuth 정보를 가져오는데 실패한 경우.</response>
    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var originHost = HttpContext.Request.Headers.Origin.FirstOrDefault("http://localhost:3000");
        return Ok(await authenticationService.LoginAsync(loginRequest, originHost!));
    }

    /// <summary>
    ///     서비스에 회원 가입합니다.
    /// </summary>
    /// <param name="signUpRequest"></param>
    /// <returns></returns>
    /// <response code="200">회원 가입에 성공했을 때 반환합니다.</response>
    /// <response code="400">JWT Join Token Validation에 실패했을 때</response>
    /// <response code="409">이미 존재하는 인증 정보일 때</response>
    [HttpPost("register")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync(SignUpRequest signUpRequest)
    {
        return Ok(await authenticationService.RegisterUserAsync(signUpRequest));
    }

    /// <summary>
    ///     리프레시 로직을 실행합니다.
    /// </summary>
    /// <param name="refreshTokenRequest">리프레시 요청</param>
    /// <returns></returns>
    /// <response code="200">리프레시에 성공했을 때 반환합니다.</response>
    /// <response code="401">엑세스 토큰이 잘못되었거나, 리프레시 로직에 실패했을 때</response>
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshAsync(RefreshTokenRequest refreshTokenRequest)
    {
        return Ok(await authenticationService.RefreshAsync(refreshTokenRequest));
    }
}
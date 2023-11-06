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
    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
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
    public async Task<IActionResult> RegisterAsync(SignUpRequest signUpRequest)
    {
        return Ok(await authenticationService.RegisterUserAsync(signUpRequest));
    }
}
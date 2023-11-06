using BlockInfrastructure.Core.Models.Requests;
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
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var originHost = HttpContext.Request.Headers.Origin.FirstOrDefault("http://localhost:3000");
        return Ok(await authenticationService.LoginAsync(loginRequest, originHost!));
    }
}
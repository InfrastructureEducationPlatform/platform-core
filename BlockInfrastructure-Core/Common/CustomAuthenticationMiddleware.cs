using System.Net.Http.Headers;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlockInfrastructure.Core.Common;

public class CustomAuthenticationMiddleware
{
    private readonly IJwtService _jwtService;
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public CustomAuthenticationMiddleware(RequestDelegate next, IJwtService jwtService,
                                          ILogger<CustomAuthenticationMiddleware> logger)
    {
        _next = next;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authenticateResult = TryAuthenticate(context);
        _logger.LogInformation("Authentication result: {0}", authenticateResult ? "Success" : "Failed");
        await _next(context);
    }

    private bool TryAuthenticate(HttpContext context)
    {
        var headerParseResult = AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out var header);

        if (!headerParseResult || header?.Parameter == null)
        {
            return false;
        }

        var jwtValidationResult = _jwtService.ValidateJwt(header.Parameter);
        if (jwtValidationResult == null)
        {
            return false;
        }

        var contextUser = new ContextUser
        {
            Email = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Email).Value,
            UserId = jwtValidationResult.Claims.First(a => a.Type == JwtRegisteredClaimNames.Sub).Value
        };
        context.SetUserContext(contextUser);
        return true;
    }
}
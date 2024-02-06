using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlockInfrastructure.Common.Configurations;
using BlockInfrastructure.Common.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlockInfrastructure.Common.Services;

public interface IJwtService
{
    public string GenerateJwt(List<Claim>? claims, DateTime? dateTime = null);

    public JwtSecurityToken? ValidateJwt(string jwt, bool validateLifetime = true);
}

public class JwtService : IJwtService
{
    private readonly ILogger<IJwtService> _logger;
    private readonly RsaSecurityKey _rsaSecurityKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtService(IOptionsMonitor<AuthConfiguration> authConfigurationMonitor, ILogger<IJwtService> logger)
    {
        _logger = logger;
        _rsaSecurityKey =
            new RsaSecurityKey(RsaExtensions.CreateRsaFromXml(authConfigurationMonitor.CurrentValue.JwtSigningKey));
        _signingCredentials = new SigningCredentials(_rsaSecurityKey, SecurityAlgorithms.RsaSha256Signature);
    }

    public string GenerateJwt(List<Claim>? claims, DateTime? dateTime = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            "BlockInfrastructure.Core",
            "BlockInfrastructure.Core",
            expires: dateTime ?? DateTime.Now.AddHours(1),
            signingCredentials: _signingCredentials,
            claims: claims
        );

        return handler.WriteToken(jwt);
    }

    public JwtSecurityToken? ValidateJwt(string jwt, bool validateLifetime = true)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _rsaSecurityKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = "BlockInfrastructure.Core",
                ValidIssuer = "BlockInfrastructure.Core",
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = validateLifetime
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "JWT Validation Failed");
            return null;
        }
    }
}
namespace BlockInfrastructure.Core.Configurations;

public class AuthConfiguration
{
    /// <summary>
    ///     JWT Signing Private (XML Format, RSA)
    /// </summary>
    public string JwtSigningKey { get; set; }

    public string GoogleOAuthClientId { get; set; }
    public string GoogleOAuthClientSecret { get; set; }
}
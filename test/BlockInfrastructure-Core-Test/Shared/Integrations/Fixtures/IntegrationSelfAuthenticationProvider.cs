using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Services.Authentication;

namespace BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;

public class IntegrationSelfAuthenticationProvider : IOAuthProviderService
{
    /// <summary>
    ///     Integration Test용 Authentication Provider 입니다.
    /// </summary>
    /// <param name="authCode">"."으로 구분된 OAuthId/Email 조합(포맷 - OAuthId.Email)</param>
    /// <param name="redirectOriginHost">Ignored</param>
    /// <returns></returns>
    public Task<OAuthResult?> GetOAuthInformationAsync(string authCode, string redirectOriginHost)
    {
        var splitResult = authCode.Split("_");
        if (splitResult.Length != 2)
        {
            return Task.FromResult<OAuthResult?>(null);
        }

        return Task.FromResult(new OAuthResult
        {
            OAuthId = splitResult[0],
            Email = splitResult[1]
        })!;
    }
}
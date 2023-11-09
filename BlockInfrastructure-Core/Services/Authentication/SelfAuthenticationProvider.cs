using BlockInfrastructure.Core.Models.Internal;

namespace BlockInfrastructure.Core.Services.Authentication;

public class SelfAuthenticationProvider : IOAuthProviderService
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
using BlockInfrastructure.Core.Models.Internal;

namespace BlockInfrastructure.Core.Services.Authentication;

public abstract class AuthenticationProviderBase : IOAuthProviderService
{
    public async Task<OAuthResult?> GetOAuthInformationAsync(string authCode, string redirectOriginHost)
    {
        // Try getting access token from OAuth Provider.
        var accessToken = await GetAccessToken(authCode, redirectOriginHost);

        // Return null if getting accessToken from provider fails.
        if (accessToken == null)
        {
            return null;
        }

        return await GetOAuthUserInformation(accessToken);
    }

    protected abstract Task<string?> GetAccessToken(string authCode, string redirectOriginHost);
    protected abstract Task<OAuthResult?> GetOAuthUserInformation(string accessToken);
}
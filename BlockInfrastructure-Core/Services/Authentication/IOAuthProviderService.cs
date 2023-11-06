using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Internal;

namespace BlockInfrastructure.Core.Services.Authentication;

public delegate IOAuthProviderService AuthProviderServiceFactory(CredentialProvider provider);

public interface IOAuthProviderService
{
    Task<OAuthResult?> GetOAuthInformationAsync(string authCode, string redirectOriginHost);
}
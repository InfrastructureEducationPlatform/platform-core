using System.Net;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services.Authentication;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public class AuthenticationService(AuthProviderServiceFactory authProviderServiceFactory, DatabaseContext databaseContext,
                                   IJwtService jwtService)
{
    public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest, string originHost)
    {
        // Get Authentication Result
        var authProvider = authProviderServiceFactory(loginRequest.Provider);
        var oAuthResult = await authProvider.GetOAuthInformationAsync(loginRequest.AuthenticationCode, originHost) ??
                          throw new ApiException(HttpStatusCode.BadRequest, "Getting OAuthInformation failed!",
                              AuthError.OAuthFailed);

        // Check Credential exists.
        var credential = await databaseContext.Credentials
                                              .Include(a => a.User)
                                              .Where(a => a.CredentialId == oAuthResult.OAuthId &&
                                                          a.CredentialProvider == loginRequest.Provider)
                                              .FirstOrDefaultAsync();

        if (credential == null)
        {
            return new TokenResponse
            {
                LoginResult = LoginResult.NeedsRegistration,
                Token = jwtService.GenerateJwtForRegistration(loginRequest.Provider, oAuthResult),
                RefreshToken = ""
            };
        }

        return new TokenResponse
        {
            LoginResult = LoginResult.LoginSucceed,
            Token = jwtService.GenerateJwtForUser(credential.User),
            RefreshToken = ""
        };
    }
}
using System.Net;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Data;
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
            RefreshToken = await GenerateRefreshTokenAsync(credential.User)
        };
    }

    public async Task<TokenResponse> RegisterUserAsync(SignUpRequest signUpRequest)
    {
        // Get Required information from token
        var (oAuthResult, provider) = jwtService.ValidateJwtForRegistration(signUpRequest.JoinToken);

        // Check Credential exists.
        var credential = await databaseContext.Credentials.Where(a => a.CredentialId == oAuthResult.OAuthId &&
                                                                      a.CredentialProvider == provider)
                                              .FirstOrDefaultAsync();
        if (credential != null)
        {
            throw new ApiException(HttpStatusCode.Conflict,
                $"Credential already exists for {oAuthResult.OAuthId}, {signUpRequest.CredentialProvider}",
                AuthError.CredentialAlreadyExists);
        }

        var user = new User
        {
            Id = Ulid.NewUlid().ToString(),
            Name = signUpRequest.Name,
            Email = signUpRequest.Email,
            ProfilePictureImageUrl = signUpRequest.ProfileImageUrl,
            CredentialList = new List<Credential>
            {
                new()
                {
                    CredentialId = oAuthResult.OAuthId,
                    CredentialProvider = provider
                }
            }
        };
        databaseContext.Users.Add(user);
        await databaseContext.SaveChangesAsync();

        return new TokenResponse
        {
            LoginResult = LoginResult.LoginSucceed,
            Token = jwtService.GenerateJwtForUser(user),
            RefreshToken = ""
        };
    }

    public async Task<TokenResponse> RefreshAsync(RefreshTokenRequest refreshRequest)
    {
        // Validate(without lifetime) Access Token
        _ = jwtService.ValidateJwt(refreshRequest.AccessToken, false) ??
            throw new ApiException(HttpStatusCode.Unauthorized, "잘못된 엑세스 토큰입니다.",
                AuthError.RefreshInvalidAccessToken);

        // Validate Refresh Token
        var refreshToken = await databaseContext.RefreshTokens
                                                .Include(refreshToken => refreshToken.User)
                                                .FirstOrDefaultAsync(a => a.Id == refreshRequest.RefreshToken) ??
                           throw new ApiException(HttpStatusCode.Unauthorized, "잘못된 리프레시 토큰입니다.",
                               AuthError.InvalidRefreshToken);

        if (refreshToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "만료된 리프레시 토큰입니다.", AuthError.RefreshExpired);
        }

        // Remove Refresh Token
        databaseContext.RefreshTokens.Remove(refreshToken);
        await databaseContext.SaveChangesAsync();

        return new TokenResponse
        {
            Token = jwtService.GenerateJwtForUser(refreshToken.User),
            RefreshToken = await GenerateRefreshTokenAsync(refreshToken.User),
            LoginResult = LoginResult.LoginSucceed
        };
    }

    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = new RefreshToken
        {
            Id = Ulid.NewUlid().ToString()!,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(14),
            User = user
        };
        databaseContext.RefreshTokens.Add(refreshToken);
        await databaseContext.SaveChangesAsync();

        return refreshToken.Id;
    }
}
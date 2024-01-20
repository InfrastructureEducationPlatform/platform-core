using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Common.Test.Fixtures;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Internal;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using BlockInfrastructure.Core.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlockInfrastructure.Core.Test.Service;

public class AuthenticationServiceTest
{
    private readonly AuthenticationService _authenticationService;
    private readonly UnitTestDatabaseContext _databaseContext;
    private readonly Mock<IOAuthProviderService> _mockAuthProviderService = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly Mock<AuthProviderServiceFactory> _mockProviderFactory = new();

    public AuthenticationServiceTest()
    {
        // Setup
        _mockProviderFactory.Setup(a => a(It.IsAny<CredentialProvider>()))
                            .Returns(_mockAuthProviderService.Object);

        // Setup DatabaseContext
        _databaseContext = new UnitTestDatabaseContext(new DbContextOptionsBuilder<DatabaseContext>()
                                                       .UseInMemoryDatabase(Ulid.NewUlid().ToString()).Options);

        // Setup SUT
        _authenticationService =
            new AuthenticationService(_mockProviderFactory.Object, _databaseContext, _mockJwtService.Object);
    }

    [Fact(DisplayName = "LoginAsync: LoginAsync는 만약 OAuth 정보를 가져오는데 실패한 경우 ApiException에 OAuthFailed 에러를 던집니다.")]
    public async Task Is_LoginAsync_Throws_ApiException_With_BadRequest_And_OAuthFailed_When_Getting_OAuth_Failed()
    {
        // Let
        var originHost = "http://localhost:3000";
        var loginRequest = new LoginRequest
        {
            Provider = CredentialProvider.Google,
            AuthenticationCode = Ulid.NewUlid().ToString()
        };
        _mockAuthProviderService.Setup(a => a.GetOAuthInformationAsync(loginRequest.AuthenticationCode, originHost))
                                .ReturnsAsync(value: null);

        // Do
        var exception =
            await Assert.ThrowsAnyAsync<ApiException>(() => _authenticationService.LoginAsync(loginRequest, originHost));

        // Verify
        _mockProviderFactory.VerifyAll();
        _mockAuthProviderService.VerifyAll();

        // Check Exception
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(AuthError.OAuthFailed.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName =
        "LoginAsync: LoginAsync는 만약 DB에 Credential이 없는 경우 LoginResult가 NeedsRegistration인 TokenResponse를 반환합니다.")]
    public async Task Is_LoginAsync_Returns_NeedsRegistration_When_Credential_Does_Not_Exists()
    {
        // Let
        var originHost = "http://localhost:3000";
        var loginRequest = new LoginRequest
        {
            Provider = CredentialProvider.Google,
            AuthenticationCode = Ulid.NewUlid().ToString()
        };
        var oAuthResult = new OAuthResult
        {
            OAuthId = Ulid.NewUlid().ToString(),
            Email = "kangdroid@test.com"
        };
        _mockAuthProviderService.Setup(a => a.GetOAuthInformationAsync(loginRequest.AuthenticationCode, originHost))
                                .ReturnsAsync(oAuthResult);
        _mockJwtService.Setup(a => a.GenerateJwt(It.IsAny<List<Claim>>(), It.IsAny<DateTime>()))
                       .Returns("jwt");

        // Do
        var tokenResponse = await _authenticationService.LoginAsync(loginRequest, originHost);

        // Verify
        _mockProviderFactory.VerifyAll();
        _mockAuthProviderService.VerifyAll();
        _mockJwtService.VerifyAll();

        // Check
        Assert.Equal(LoginResult.NeedsRegistration, tokenResponse.LoginResult);
        Assert.Equal("jwt", tokenResponse.Token);
        Assert.Equal("", tokenResponse.RefreshToken);
    }

    [Fact(DisplayName = "LoginAsync: LoginAsync는 만약 DB에 Credential이 있는 경우 LoginResult가 LoginSucceed인 TokenResponse를 반환합니다.")]
    public async Task Is_LoginAsync_Returns_LoginSucceed_When_Credential_Exists()
    {
        // Let
        var originHost = "http://localhost:3000";
        var loginRequest = new LoginRequest
        {
            Provider = CredentialProvider.Google,
            AuthenticationCode = Ulid.NewUlid().ToString()
        };
        var oAuthResult = new OAuthResult
        {
            OAuthId = Ulid.NewUlid().ToString(),
            Email = "kangdroid@test.com"
        };
        var credential = new Credential
        {
            CredentialId = oAuthResult.OAuthId,
            CredentialProvider = CredentialProvider.Google,
            User = new User
            {
                Id = Ulid.NewUlid().ToString(),
                Email = "kangdroid@test.com",
                Name = "asdf"
            }
        };
        _mockAuthProviderService.Setup(a => a.GetOAuthInformationAsync(loginRequest.AuthenticationCode, originHost))
                                .ReturnsAsync(oAuthResult);
        _mockJwtService.Setup(a => a.GenerateJwt(It.IsAny<List<Claim>>(), It.IsAny<DateTime>()))
                       .Returns("jwt");
        _databaseContext.Credentials.Add(credential);
        await _databaseContext.SaveChangesAsync();

        // Do
        var tokenResponse = await _authenticationService.LoginAsync(loginRequest, originHost);

        // Verify
        _mockProviderFactory.VerifyAll();
        _mockAuthProviderService.VerifyAll();
        _mockJwtService.VerifyAll();

        // Check
        Assert.Equal(LoginResult.LoginSucceed, tokenResponse.LoginResult);
        Assert.Equal("jwt", tokenResponse.Token);
        Assert.NotEmpty(tokenResponse.RefreshToken);
    }

    [Fact(DisplayName = "RegisterUserAsync: RegisterUserAsync는 만약 DB에 Credential이 이미 존재하는 경우 ApiException을 던집니다.")]
    public async Task Is_RegisterUserAsync_Throws_ApiException_When_Credential_Already_Exists()
    {
        // Let
        var signUpRequest = new SignUpRequest
        {
            Name = "KangDroid",
            Email = "kangdroid@test.com",
            ProfileImageUrl = null,
            JoinToken = Ulid.NewUlid().ToString(),
            CredentialProvider = CredentialProvider.Google
        };
        var oAuthResult = new OAuthResult
        {
            OAuthId = Ulid.NewUlid().ToString(),
            Email = "kangdroid@test.com"
        };
        var credential = new Credential
        {
            CredentialId = oAuthResult.OAuthId,
            CredentialProvider = CredentialProvider.Google,
            User = new User
            {
                Id = Ulid.NewUlid().ToString(),
                Email = "kangdroid@test.com",
                Name = "asdf"
            }
        };
        _databaseContext.Credentials.Add(credential);
        await _databaseContext.SaveChangesAsync();
        _mockJwtService.Setup(a => a.ValidateJwt(It.IsAny<string>(), true))
                       .Returns(new JwtSecurityToken(claims: new List<Claim>
                       {
                           new("provider", CredentialProvider.Google.ToString()),
                           new("sub", oAuthResult.OAuthId),
                           new(JwtRegisteredClaimNames.Email, oAuthResult.Email)
                       }));

        // Do
        var exception =
            await Assert.ThrowsAnyAsync<ApiException>(() => _authenticationService.RegisterUserAsync(signUpRequest));

        // Verify
        _mockJwtService.VerifyAll();

        // Check
        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Equal(AuthError.CredentialAlreadyExists.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName =
        "RegisterUserAsync: RegisterUserAsync는 만약 Credential이 없으면 사용자를 저장하고 LoginSucceed타입이 들어간 TokenResponse를 반환합니다.")]
    public async Task Is_RegisterUserAsync_Returns_LoginSucceed_When_Credential_Does_Not_Exists()
    {
        // Let
        var signUpRequest = new SignUpRequest
        {
            Name = "KangDroid",
            Email = "kangdroid@test.com",
            ProfileImageUrl = null,
            JoinToken = Ulid.NewUlid().ToString(),
            CredentialProvider = CredentialProvider.Google
        };
        var oAuthResult = new OAuthResult
        {
            OAuthId = Ulid.NewUlid().ToString(),
            Email = "kangdroid@test.com"
        };
        _mockJwtService.Setup(a => a.ValidateJwt(It.IsAny<string>(), true))
                       .Returns(new JwtSecurityToken(claims: new List<Claim>
                       {
                           new("provider", CredentialProvider.Google.ToString()),
                           new("sub", oAuthResult.OAuthId),
                           new(JwtRegisteredClaimNames.Email, oAuthResult.Email)
                       }));
        _mockJwtService.Setup(a => a.GenerateJwt(It.IsAny<List<Claim>>(), It.IsAny<DateTime>()))
                       .Returns("jwt");

        // Do
        var tokenResponse = await _authenticationService.RegisterUserAsync(signUpRequest);

        // Verify
        _mockJwtService.VerifyAll();

        // Check
        Assert.Equal(LoginResult.LoginSucceed, tokenResponse.LoginResult);
        Assert.Equal("jwt", tokenResponse.Token);
        Assert.Equal("", tokenResponse.RefreshToken);
    }

    [Fact(DisplayName =
        "RefreshAsync: RefreshAsync는 만약 엑세스 토큰이 잘못된 경우 ApiException에 AuthError.RefreshInvalidAccessToken을 던집니다.")]
    public async Task Is_RefreshAsync_Throws_ApiException_With_RefreshInvalidAccessToken_When_AccessToken_Invalid()
    {
        // Let
        var request = new RefreshTokenRequest
        {
            AccessToken = Ulid.NewUlid().ToString(),
            RefreshToken = ""
        };
        _mockJwtService.Setup(a => a.ValidateJwt(request.AccessToken, false))
                       .Returns(value: null);

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _authenticationService.RefreshAsync(request));

        // Check Exception
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(AuthError.RefreshInvalidAccessToken.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName =
        "RefreshAsync: RefreshAsync는 만약 리프레시 토큰을 찾을 수 없는 경우 ApiException에 AuthError.InvalidRefreshToken을 던집니다.")]
    public async Task Is_RefreshAsync_Throws_ApiException_With_InvalidRefreshToken_When_RefreshToken_Not_Found()
    {
        // Let
        var request = new RefreshTokenRequest
        {
            AccessToken = Ulid.NewUlid().ToString(),
            RefreshToken = Ulid.NewUlid().ToString()
        };
        _mockJwtService.Setup(a => a.ValidateJwt(request.AccessToken, false))
                       .Returns(new JwtSecurityToken());
        _databaseContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Ulid.NewUlid().ToString(),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
            User = new User
            {
                Id = Ulid.NewUlid().ToString(),
                Email = "kangdroid@test.com",
                Name = "asdf"
            }
        });
        await _databaseContext.SaveChangesAsync();

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _authenticationService.RefreshAsync(request));

        // Check Exception
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(AuthError.InvalidRefreshToken.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "RefreshAsync: RefreshAsync는 만약 리프레시 토큰이 만료된 경우 ApiException에 AuthError.RefreshExpired을 던집니다.")]
    public async Task Is_RefreshAsync_Throws_ApiException_With_RefreshExpired_When_RefreshToken_Expired()
    {
        // Let
        var request = new RefreshTokenRequest
        {
            AccessToken = Ulid.NewUlid().ToString(),
            RefreshToken = "tes"
        };
        _mockJwtService.Setup(a => a.ValidateJwt(request.AccessToken, false))
                       .Returns(new JwtSecurityToken());
        _databaseContext.RefreshTokens.Add(new RefreshToken
        {
            Id = "tes",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
            User = new User
            {
                Id = Ulid.NewUlid().ToString(),
                Email = "kangdroid@test.com",
                Name = "asdf"
            }
        });
        await _databaseContext.SaveChangesAsync();

        // Do
        var exception = await Assert.ThrowsAnyAsync<ApiException>(() => _authenticationService.RefreshAsync(request));

        // Check Exception
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(AuthError.RefreshExpired.ErrorTitleToString(), exception.ErrorTitle.ErrorTitleToString());
    }

    [Fact(DisplayName = "RefreshAsync: RefreshAsync는 만약 리프레시 토큰이 만료되지 않은 경우 새로운 엑세스 토큰을 발급합니다.")]
    public async Task Is_RefreshAsync_Returns_New_AccessToken_When_RefreshToken_Not_Expired()
    {
        // Let
        var request = new RefreshTokenRequest
        {
            AccessToken = Ulid.NewUlid().ToString(),
            RefreshToken = "test"
        };
        _mockJwtService.Setup(a => a.ValidateJwt(request.AccessToken, false))
                       .Returns(new JwtSecurityToken());
        _mockJwtService.Setup(a => a.GenerateJwt(It.IsAny<List<Claim>>(), It.IsAny<DateTime>()))
                       .Returns("jwt");

        _databaseContext.RefreshTokens.Add(new RefreshToken
        {
            Id = "test",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(10),
            User = new User
            {
                Id = Ulid.NewUlid().ToString(),
                Email = "kangdroid@test.com",
                Name = "asdf"
            }
        });
        await _databaseContext.SaveChangesAsync();

        // Do
        var tokenResponse = await _authenticationService.RefreshAsync(request);

        // Verify
        _mockJwtService.VerifyAll();

        // Check Response
        Assert.Equal("jwt", tokenResponse.Token);
        Assert.NotEmpty(tokenResponse.RefreshToken);
        Assert.Single(_databaseContext.RefreshTokens);
    }
}
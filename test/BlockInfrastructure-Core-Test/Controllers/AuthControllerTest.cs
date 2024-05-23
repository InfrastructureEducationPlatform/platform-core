using System.Net;
using System.Net.Http.Json;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class AuthControllerTest(ContainerFixture containerFixture, ITestOutputHelper outputHelper)
    : IntegrationsTestHelper(containerFixture, outputHelper)
{
    [Fact(DisplayName = "POST /auth/login: LoginAsync은 OAuth 인증 정보를 가져오는데 실패한 경우 400 BadRequest를 반환합니다.")]
    public async Task Is_LoginAsync_Returns_BadRequest_When_OAuth_Request_Failed()
    {
        // Let
        var loginRequest = new LoginRequest
        {
            AuthenticationCode = Ulid.NewUlid().ToString(),
            Provider = CredentialProvider.Google
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/login", loginRequest);

        // Check HTTP Status Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Check Error Response
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(errorResponse);
        Assert.Equal(AuthError.OAuthFailed.ErrorTitleToString(), errorResponse.ErrorTitle);
    }

    [Fact(DisplayName = "POST /auth/login: LoginAsync는 만약 일치하는 사용자가 없다면 200 OK 와 함께 NeedsRegistration 응답을 반환합니다.")]
    public async Task Is_LoginAsync_Returns_200_Ok_With_NeedsRegistration_When_No_Credential_Match()
    {
        // Let
        var loginRequest = new LoginRequest
        {
            AuthenticationCode = $"{Ulid.NewUlid().ToString()}_kangdroid@test.com",
            Provider = CredentialProvider.Google
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/login", loginRequest);

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Token Response
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(tokenResponse);
        Assert.Equal(LoginResult.NeedsRegistration, tokenResponse.LoginResult);
        Assert.NotEmpty(tokenResponse.Token);
    }

    [Fact(DisplayName = "POST /auth/login: LoginAsync는 만약 일치하는 사용자가 있는 경우 200 OK와 함께 LoginSucceed를 반환합니다.")]
    public async Task Is_LoginAsync_Returns_200_Ok_With_LoginSucceed_When_User_Exists()
    {
        // Let
        var (user, tokenResponse) = await CreateAccountAsync();
        var loginRequest = new LoginRequest
        {
            AuthenticationCode = $"{user.CredentialList.First().CredentialId}_{user.Email}",
            Provider = CredentialProvider.Google
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/login", loginRequest);

        // Check HTTP Status Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Token Response
        var tokenResponseFromServer = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(tokenResponseFromServer);
        Assert.Equal(LoginResult.LoginSucceed, tokenResponseFromServer.LoginResult);
        Assert.NotEmpty(tokenResponseFromServer.Token);
        Assert.NotNull(tokenResponseFromServer.RefreshToken);
        Assert.NotEmpty(tokenResponseFromServer.RefreshToken);
    }

    [Fact(DisplayName = "POST /auth/refresh: RefreshAsync는 만약 엑세스 토큰이 잘못된 경우 401을 반환합니다.")]
    public async Task Is_RefreshAsync_Returns_401_When_AccessToken_Invalid()
    {
        // Let
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = Ulid.NewUlid().ToString(),
            RefreshToken = "asdf"
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/refresh", refreshRequest);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /auth/refresh: RefreshAsync는 만약 리프레시 토큰이 잘못된 경우 401을 반환합니다.")]
    public async Task Is_RefreshAsync_Returns_401_When_RefreshToken_Invalid()
    {
        // Let
        var (user, tokenResponse) = await CreateAccountAsync();
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = tokenResponse.Token,
            RefreshToken = "asdf"
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/refresh", refreshRequest);

        // Check
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "POST /auth/refresh: RefreshAsync는 만약 정상적인 요청인 경우 200 OK를 반환합니다.")]
    public async Task Is_RefreshAsync_Returns_200_When_Request_Valid()
    {
        // Let
        var (user, tokenResponse) = await CreateAccountAsync();
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = tokenResponse.Token,
            RefreshToken = tokenResponse.RefreshToken
        };

        // Do
        var response = await WebRequestClient.PostAsJsonAsync("/auth/refresh", refreshRequest);

        // Check
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
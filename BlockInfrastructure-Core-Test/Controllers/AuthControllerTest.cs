using System.Net;
using System.Net.Http.Json;
using BlockInfrastructure.Core.Common.Errors;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Test.Shared.Integrations;
using BlockInfrastructure.Core.Test.Shared.Integrations.Fixtures;
using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class AuthControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
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
        Assert.Empty(tokenResponseFromServer.RefreshToken);
    }
}
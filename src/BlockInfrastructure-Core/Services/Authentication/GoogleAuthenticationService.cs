using System.Net.Http.Headers;
using BlockInfrastructure.Common.Configurations;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockInfrastructure.Core.Services.Authentication;

public class GoogleAuthenticationService(
    IOptionsMonitor<AuthConfiguration> authConfigOptions,
    IHttpClientFactory clientFactory,
    ILogger<GoogleAuthenticationService> logger)
    : AuthenticationProviderBase
{
    private readonly string _clientId = authConfigOptions.CurrentValue.GoogleOAuthClientId;
    private readonly string _clientSecret = authConfigOptions.CurrentValue.GoogleOAuthClientSecret;
    private readonly ILogger _logger = logger;

    protected async override Task<string?> GetAccessToken(string authCode, string redirectOriginHost)
    {
        // Send AccessToken Request
        var httpClient = clientFactory.CreateClient(HttpClientNames.GoogleOAuthApi);
        var response = await httpClient.PostAsync("/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = $"{redirectOriginHost}/auth/callback",
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["code"] = authCode
        }));

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get access token: [Google]: {responseBody}",
                await response.Content.ReadAsStringAsync());
            return null;
        }

        var accessTokenResponse = await response.Content.Deserialize<GoogleAccessTokenResponse>();
        return accessTokenResponse?.AccessToken;
    }

    protected async override Task<OAuthResult?> GetOAuthUserInformation(string accessToken)
    {
        // Create HttpClient for Google API(Not OAuth Api)
        var httpClient = clientFactory.CreateClient(HttpClientNames.GoogleApi);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync("/oauth2/v2/userinfo");

        // When Google's response is not successful, return null
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get OAuth User Information: [Google]: {responseBody}",
                await response.Content.ReadAsStringAsync());
            return null;
        }

        // Get Response
        var googleMeResponse = await response.Content.Deserialize<GoogleMeResponse>();

        return new OAuthResult
        {
            OAuthId = googleMeResponse.Id,
            Email = googleMeResponse.Email
        };
    }

    private class GoogleAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    private class GoogleMeResponse
    {
        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
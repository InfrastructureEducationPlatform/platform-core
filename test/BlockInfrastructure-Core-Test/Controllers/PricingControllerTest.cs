using System.Net;
using System.Net.Http.Headers;
using BlockInfrastructure.Common.Test.Shared.Integrations;
using BlockInfrastructure.Common.Test.Shared.Integrations.Fixtures;

namespace BlockInfrastructure.Core.Test.Controllers;

[Collection("Container")]
public class PricingControllerTest(ContainerFixture containerFixture) : IntegrationsTestHelper(containerFixture)
{
    [Fact(DisplayName =
        "GET /pricing: GetAllPricingInformationAsync는 만약 인증되지 않은 사용자가 요청할 경우 401 Unauthorized를 반환합니다.")]
    public async Task Is_GetAllPricingInformationAsync_Returns_401_Unauthorized_When_Unauthenticated_User_Requested()
    {
        // Do
        var response = await WebRequestClient.GetAsync("/pricing");

        // Check
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "GET /pricing: GetAllPricingInformationAsync는 만약 정상적인 요청인 경우 가격 정보를 반환합니다.")]
    public async Task Is_GetAllPricingInformationAsync_Returns_PricingInformation_When_Authenticated_User_Requested()
    {
        // Let
        var (user, tokenResponse) = await CreateAccountAsync();
        WebRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // Do
        var response = await WebRequestClient.GetAsync("/pricing");

        // Check
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
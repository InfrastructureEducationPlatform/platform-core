using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Responses;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/pricing")]
public class PricingController(PricingService pricingService) : ControllerBase
{
    /// <summary>
    ///     서버에 등록된 모든 가격 정보를 불러옵니다.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">성공적으로 가격 정보를 불러왔습니다.</response>
    /// <response code="401">인증 토큰이 없어 인증에 실패한 경우</response>
    [HttpGet]
    [JwtAuthenticationFilter]
    [ProducesResponseType<List<PricingInformation>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllPricingInformationAsync()
    {
        return Ok(await pricingService.GetAllPricingInformationAsync());
    }
}
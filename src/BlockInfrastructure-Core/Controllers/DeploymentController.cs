using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/deployment")]
public class DeploymentController(DeploymentService deploymentService) : ControllerBase
{
    /// <summary>
    ///     특정 배포 정보를 가져옵니다.(FE Polling혹은 배포 정보 조회)
    /// </summary>
    /// <remarks>
    ///     해당 API는 과도한 DB조회를 막기 위해 최초 요청으로부터 1분간 캐싱합니다.
    /// </remarks>
    /// <param name="deploymentId">조회할 배포 정보 ID</param>
    /// <returns></returns>
    /// <response code="200">배포 정보를 성공적으로 가져온 경우</response>
    /// <response code="401">인증이 실패한 경우</response>
    /// <response code="404">배포 정보를 찾을 수 없는 경우</response>
    [JwtAuthenticationFilter]
    [HttpGet("{deploymentId}")]
    [ProducesResponseType<DeploymentProjection>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeploymentInformationAsync(string deploymentId)
    {
        var deploymentLog = await deploymentService.GetDeploymentAsync(deploymentId);
        return Ok(DeploymentProjection.FromDeploymentLog(deploymentLog));
    }
}
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Common.Extensions;
using BlockInfrastructure.Core.Models.MediatR.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[Route("/deployment")]
public class DeploymentController(IDeploymentService deploymentService, IMediator mediator) : ControllerBase
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

    /// <summary>
    ///     특정 사용자에 해당하는 배포 정보 상태들을 모두 불러옵니다.
    /// </summary>
    /// <remarks>해당 API는 사용자가 소속되어 있는 채널의 모든 배포 리스트를 불러옵니다.</remarks>
    /// <returns></returns>
    /// <response code="200">배포 정보를 성공적으로 가져온 경우</response>
    /// <response code="401">인증이 실패한 경우</response>
    [HttpGet]
    [JwtAuthenticationFilter]
    [ProducesResponseType<List<DeploymentProjection>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDeploymentInformationListAsync()
    {
        var userContext = HttpContext.GetUserContext();
        var response = await mediator.Send(new GetDeploymentInformationListForUser
        {
            ContextUser = userContext
        });
        return Ok(response.Select(DeploymentProjection.FromDeploymentLog));
    }
}
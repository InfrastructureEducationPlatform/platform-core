using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using BlockInfrastructure.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Core.Controllers;

[ApiController]
[JwtAuthenticationFilter]
[Route("/channels/{channelId}/sketches")]
public class SketchController(SketchService sketchService) : ControllerBase
{
    /// <summary>
    ///     채널 내에 있는 모든 스케치를 가져옵니다,
    /// </summary>
    /// <remarks>
    ///     이 API는 채널의 Owner, Reader 모두 조회할 수 있습니다.
    /// </remarks>
    /// <param name="channelId">조회할 채널 ID</param>
    /// <returns></returns>
    /// <response code="200">조회에 성공한 경우.</response>
    /// <response code="401">인증 토큰이 없어 인증에 실패한 경우</response>
    /// <response code="403">스케치를 가져오는데 채널 권한이 부족한 경우(해당 API에서는 소속되어 있지 않은 채널을 조회하려 했을 때 반환)</response>
    [HttpGet]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
    [ProducesResponseType<List<SketchResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListSketchesInChannelAsync(string channelId)
    {
        return Ok(await sketchService.ListSketches(channelId));
    }

    /// <summary>
    ///     채널 내에 빈 스케치를 새로 추가합니다.
    /// </summary>
    /// <remarks>
    ///     이 API는 채널의 Owner만 사용할 수 있습니다.
    /// </remarks>
    /// <param name="channelId">채널 ID</param>
    /// <param name="createSketchRequest">스케치 생성 요청</param>
    /// <returns></returns>
    /// <response code="200">스케치 생성에 성공한 경우.</response>
    /// <response code="403">스케치를 생성하는데 채널 권한이 부족한 경우(해당 API에서는 owner권한만 허용.)</response>
    [HttpPost]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    [ProducesResponseType<SketchResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSketchAsync(string channelId, CreateSketchRequest createSketchRequest)
    {
        return Ok(await sketchService.CreateSketchAsync(channelId, createSketchRequest));
    }

    /// <summary>
    ///     스케치를 업데이트 합니다.
    /// </summary>
    /// <param name="channelId">채널 ID</param>
    /// <param name="sketchId">업데이트 할 스케치 ID</param>
    /// <param name="updateSketchRequest">업데이트 할 스케치 내용</param>
    /// <returns></returns>
    [HttpPut("{sketchId}")]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    [ProducesResponseType<SketchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSketchAsync(string channelId, string sketchId,
                                                       UpdateSketchRequest updateSketchRequest)
    {
        return Ok(await sketchService.UpdateSketchAsync(channelId, sketchId, updateSketchRequest));
    }

    /// <summary>
    ///     특정 스케치를 불러옵니다.
    /// </summary>
    /// <param name="channelId">채널 ID</param>
    /// <param name="sketchId">가져올 특정 스케치 ID</param>
    /// <returns></returns>
    /// <response code="200">정상적으로 데이터를 불러왔을 때</response>
    /// <response code="404">해당 스케치를 찾을 수 없을 때</response>
    [HttpGet("{sketchId}")]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
    [ProducesResponseType<SketchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSketchAsync(string channelId, string sketchId)
    {
        return Ok(await sketchService.GetSketchAsync(channelId, sketchId));
    }

    /// <summary>
    ///     배포를 시작합니다.
    /// </summary>
    /// <remarks>
    ///     해당 API는 Blocking API가 아닌, Non-Blocking API이며, 응답으로는 배포의 상황을 조회할 수 있는 DeploymentProjection을 반환합니다.
    /// </remarks>
    /// <param name="sketchId"></param>
    /// <returns></returns>
    /// <response code="202">정상적으로 배포 시작에 성공한 경우</response>
    /// <response code="404">해당 스케치를 찾을 수 없을 때</response>
    [HttpPost("{sketchId}/deploy")]
    [ChannelRole(ChannelIdGetMode.Route, "channelId", ChannelPermissionType.Owner)]
    [ProducesResponseType<DeploymentProjection>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeploySketchAsync(string sketchId)
    {
        var deploymentLog = await sketchService.DeployAsync(sketchId);
        return Accepted(DeploymentProjection.FromDeploymentLog(deploymentLog));
    }
}
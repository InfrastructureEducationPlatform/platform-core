using BlockInfrastructure.Core.Common;
using BlockInfrastructure.Core.Models.Data;
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
    [ChannelRole(ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
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
    [ChannelRole(ChannelPermissionType.Owner)]
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
    [ChannelRole(ChannelPermissionType.Owner)]
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
    [ChannelRole(ChannelPermissionType.Owner, ChannelPermissionType.Reader)]
    [ProducesResponseType<SketchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSketchAsync(string channelId, string sketchId)
    {
        return Ok(await sketchService.GetSketchAsync(channelId, sketchId));
    }

    [HttpPost("{sketchId}/deploy")]
    [ChannelRole(ChannelPermissionType.Owner)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TempDeployment(string sketchId)
    {
        await sketchService.TempDeployAsync(sketchId);
        return Ok();
    }
}
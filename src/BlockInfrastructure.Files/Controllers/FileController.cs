using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Files.Models.Responses;
using BlockInfrastructure.Files.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlockInfrastructure.Files.Controllers;

[ApiController]
[Route("/files")]
public class FileController(FileService fileService) : ControllerBase
{
    /// <summary>
    ///     파일을 업로드 합니다.
    /// </summary>
    /// <param name="formFile">파일 업로드(FormFile)</param>
    /// <returns></returns>
    [JwtAuthenticationFilter]
    [HttpPost("upload")]
    [ProducesResponseType<FileProjectionResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadFileAsync(IFormFile formFile)
    {
        var contextUser = HttpContext.GetUserContext();
        var fileProjectionResponse = await fileService.UploadFileAsync(formFile, contextUser);
        return Ok(fileProjectionResponse);
    }

    /// <summary>
    ///     파일을 가져옵니다.
    /// </summary>
    /// <param name="fileId">파일 Id</param>
    /// <returns></returns>
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFileAsync(string fileId)
    {
        var file = await fileService.GetFileAsync(fileId);
        return File(file.BinaryData, file.ContentType, file.ContentDispositionFileName);
    }
}
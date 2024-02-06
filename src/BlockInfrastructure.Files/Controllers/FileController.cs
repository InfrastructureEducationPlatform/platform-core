using BlockInfrastructure.Common.Extensions;
using BlockInfrastructure.Common.Services;
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
    public async Task<IActionResult> UploadFileAsync(IFormFile formFile)
    {
        var contextUser = HttpContext.GetUserContext();
        var fileProjectionResponse = await fileService.UploadFileAsync(formFile, contextUser);
        return Ok(fileProjectionResponse);
    }
}
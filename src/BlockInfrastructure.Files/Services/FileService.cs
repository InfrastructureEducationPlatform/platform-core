using System.Net;
using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Files.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace BlockInfrastructure.Files.Services;

public class FileService(FileTypeVerifierService fileTypeVerifierService, DatabaseContext databaseContext)
{
    public async Task<FileProjectionResponse> UploadFileAsync(IFormFile formFile, ContextUser contextUser)
    {
        // Pre Validate
        var determinedFileType = fileTypeVerifierService.Determine(formFile.OpenReadStream());
        if (determinedFileType.Name == "Unknown")
        {
            throw new ApiException(HttpStatusCode.BadRequest, "File type is not supported", FileError.FileFormatNotSupported);
        }

        // Create File Object
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        var file = new BlockFile
        {
            ContentType = formFile.ContentType,
            ContentDispositionName = formFile.Name,
            ContentDispositionFileName = formFile.FileName,
            UserId = contextUser.UserId,
            UserEmail = contextUser.Email,
            BinaryData = memoryStream.ToArray(),
            FileSize = memoryStream.Length
        };

        // Save to Database
        databaseContext.BlockFiles.Add(file);
        await databaseContext.SaveChangesAsync();

        return FileProjectionResponse.FromBlockFile(file);
    }

    public async Task<BlockFile> GetFileAsync(string fileId)
    {
        var file = await databaseContext.BlockFiles.FindAsync(fileId);
        if (file == null)
        {
            throw new ApiException(HttpStatusCode.NotFound, "File not found", FileError.FileNotFound);
        }

        return file;
    }
}
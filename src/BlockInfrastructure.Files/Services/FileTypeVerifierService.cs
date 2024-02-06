using BlockInfrastructure.Files.Models.Internal;
using BlockInfrastructure.Files.Models.Internal.FileExtensions;

namespace BlockInfrastructure.Files.Services;

public class FileTypeVerifierService
{
    private readonly IEnumerable<FileType> Types = new List<FileType>
    {
        new Jpeg(),
        new Png()
    }.OrderByDescending(a => a.SignatureLength);

    private readonly FileTypeVerifyResult Unknown = new()
    {
        Name = "Unknown",
        Description = "Unknown File Type",
        IsVerified = false
    };

    public FileTypeVerifyResult Determine(Stream stream)
    {
        FileTypeVerifyResult? result = null;

        foreach (var fileType in Types)
        {
            result = fileType.Verify(stream);
            if (result.IsVerified)
            {
                break;
            }
        }

        return result?.IsVerified == true
            ? result
            : Unknown;
    }
}
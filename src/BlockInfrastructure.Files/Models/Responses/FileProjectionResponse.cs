using System.ComponentModel.DataAnnotations;
using BlockInfrastructure.Common.Models.Data;

namespace BlockInfrastructure.Files.Models.Responses;

public class FileProjectionResponse
{
    [Required]
    public string FileId { get; set; }

    [Required]
    public string ContentDispositionFileName { get; set; }

    [Required]
    public string ContentDispositionName { get; set; }

    [Required]
    public string ContentType { get; set; }

    [Required]
    public long FileSize { get; set; }


    public static FileProjectionResponse FromBlockFile(BlockFile blockFile)
    {
        return new FileProjectionResponse
        {
            FileId = blockFile.Id,
            ContentDispositionFileName = blockFile.ContentDispositionFileName,
            ContentDispositionName = blockFile.ContentDispositionName,
            ContentType = blockFile.ContentType,
            FileSize = blockFile.FileSize
        };
    }
}
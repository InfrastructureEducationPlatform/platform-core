using System.ComponentModel.DataAnnotations;

namespace BlockInfrastructure.Common.Models.Data;

public class BlockFile : AutomaticAuditSupport
{
    [Key]
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    public string ContentDispositionFileName { get; set; }

    public string ContentDispositionName { get; set; }

    public string ContentType { get; set; }

    public string UserId { get; set; }

    public string UserEmail { get; set; }

    public long FileSize { get; set; }

    public byte[] BinaryData { get; set; }
}
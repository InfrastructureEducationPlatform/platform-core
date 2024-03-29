using System.Text;

namespace BlockInfrastructure.Files.Models.Internal;

public abstract class FileType
{
    protected string Description { get; set; }
    protected string Name { get; set; }

    private List<string> Extensions { get; } = new();

    private List<byte[]> Signatures { get; } = new();

    public int SignatureLength => Signatures.Max(m => m.Length);

    protected FileType AddSignatures(params byte[][] bytes)
    {
        Signatures.AddRange(bytes);
        return this;
    }

    protected FileType AddExtensions(params string[] extensions)
    {
        Extensions.AddRange(extensions);
        return this;
    }

    public FileTypeVerifyResult Verify(Stream stream)
    {
        stream.Position = 0;
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);
        var headerBytes = reader.ReadBytes(SignatureLength);

        return new FileTypeVerifyResult
        {
            Name = Name,
            Description = Description,
            IsVerified = Signatures.Any(signature =>
                headerBytes.Take(signature.Length)
                           .SequenceEqual(signature)
            )
        };
    }
}
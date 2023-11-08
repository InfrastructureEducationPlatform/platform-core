using System.Text;
using System.Text.Json;
using BlockInfrastructure.Core.Models.Data;
using BlockInfrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Test.Fixtures;

public class UnitTestDatabaseContext : DatabaseContext
{
    public UnitTestDatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DeploymentLog>().Property(p => p.DeploymentOutput)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));
        modelBuilder.Entity<Plugin>()
                    .Property(a => a.SamplePluginConfiguration)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));
        modelBuilder.Entity<PluginInstallation>()
                    .Property(a => a.PluginConfiguration)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));
        modelBuilder.Entity<Sketch>()
                    .Property(a => a.BlockSketch)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));

        modelBuilder.Entity<RequestLog>()
                    .Property(a => a.RequestBody)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));

        modelBuilder.Entity<RequestLog>()
                    .Property(a => a.ResponseBody)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));

        modelBuilder.Entity<RequestLog>()
                    .Property(a => a.RequestHeaders)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));
        modelBuilder.Entity<RequestLog>()
                    .Property(a => a.ResponseHeaders)
                    .HasConversion(
                        v => JsonDocumentToString(v),
                        v => JsonDocument.Parse(v, new JsonDocumentOptions()));
    }

    private static string JsonDocumentToString(JsonDocument document)
    {
        using var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = true
        });
        document.WriteTo(writer);
        writer.Flush();
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
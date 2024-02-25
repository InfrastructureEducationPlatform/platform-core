using BlockInfrastructure.Common.Models.Data;
using BlockInfrastructure.Common.Models.Internal.PluginConfigs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlockInfrastructure.Common.Services;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<ChannelPermission> ChannelPermissions { get; set; }
    public DbSet<Sketch> Sketches { get; set; }
    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<PluginInstallation> PluginInstallations { get; set; }
    public DbSet<DeploymentLog> DeploymentLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BlockFile> BlockFiles { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>()
                    .HasKey(a => new
                    {
                        a.CredentialId,
                        a.CredentialProvider
                    });
        modelBuilder.Entity<Credential>()
                    .Property(a => a.CredentialProvider)
                    .HasConversion<string>();

        modelBuilder.Entity<ChannelPermission>()
                    .HasKey(a => new
                    {
                        a.ChannelId,
                        a.UserId
                    });
        modelBuilder.Entity<ChannelPermission>()
                    .Property(a => a.ChannelPermissionType)
                    .HasConversion<string>();

        modelBuilder.Entity<Plugin>()
                    .Property(a => a.SamplePluginConfiguration)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<Plugin>()
                    .Property(a => a.PluginTypeDefinitions)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<PluginTypeDefinition>>(v) ?? new List<PluginTypeDefinition>());

        // Default Data Seed
        modelBuilder.Entity<Plugin>()
                    .HasData(new Plugin
                    {
                        Id = "aws-static",
                        Name = "Amazon Static Credential Provider Plugin",
                        Description = "Amazon Access Key를 사용하는 Credential Provider Plugin",
                        SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new AwsStaticProviderConfig
                        {
                            AccessKey = "Access Key ID",
                            SecretKey = "Access Secret Key",
                            Region = "Default Region Code(i.e: ap-northeast-2)"
                        }),
                        CreatedAt = DateTimeOffset.Parse("2024-02-25T00:00:00Z"),
                        UpdatedAt = DateTimeOffset.Parse("2024-02-25T00:00:00Z"),
                        PluginTypeDefinitions = new List<PluginTypeDefinition>
                        {
                            new()
                            {
                                FieldName = "AccessKey",
                                FieldType = "string",
                                FieldDescription = "AWS Access Key",
                                IsRequired = true,
                                DefaultValue = ""
                            },
                            new()
                            {
                                FieldName = "SecretKey",
                                FieldType = "string",
                                FieldDescription = "AWS Secret Key",
                                IsRequired = true,
                                DefaultValue = ""
                            },
                            new()
                            {
                                FieldName = "Region",
                                FieldType = "string",
                                FieldDescription = "AWS Region",
                                IsRequired = true,
                                DefaultValue = "ap-northeast-2"
                            }
                        }
                    });

        modelBuilder.Entity<PluginInstallation>()
                    .HasKey(a => new
                    {
                        a.ChannelId,
                        a.PluginId
                    });
        modelBuilder.Entity<PluginInstallation>()
                    .Property(a => a.PluginConfiguration)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<DeploymentLog>()
                    .Property(a => a.DeploymentStatus)
                    .HasConversion<string>();
        modelBuilder.Entity<DeploymentLog>()
                    .Property(a => a.DeploymentOutput)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<Sketch>()
                    .Property(a => a.BlockSketch)
                    .HasColumnType("jsonb");

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        AuditData();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        AuditData();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AuditData()
    {
        foreach (var eachEntry in ChangeTracker.Entries())
        {
            var support = eachEntry.Entity as AutomaticAuditSupport;
            if (support == null)
            {
                continue;
            }

            var currentTime = DateTimeOffset.UtcNow;
            switch (eachEntry.State)
            {
                case EntityState.Added:
                    support.CreatedAt = currentTime;
                    support.UpdatedAt = currentTime;
                    break;
                case EntityState.Modified:
                    support.UpdatedAt = currentTime;
                    break;
            }
        }
    }
}
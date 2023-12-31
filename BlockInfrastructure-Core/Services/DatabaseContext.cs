using BlockInfrastructure.Core.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

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
    public DbSet<RequestLog> RequestLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


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

        modelBuilder.Entity<RequestLog>()
                    .Property(a => a.RequestBody)
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
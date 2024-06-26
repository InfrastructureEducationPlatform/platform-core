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
    public DbSet<UserAction> UserActions { get; set; }

    public DbSet<PricingInformation> PricingInformations { get; set; }
    public DbSet<PriceInfoPerVendor> PriceInfoPerVendors { get; set; }

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
                    .HasData(new List<Plugin>
                    {
                        new()
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
                                    DefaultValue = "",
                                    IsSecret = false
                                },
                                new()
                                {
                                    FieldName = "SecretKey",
                                    FieldType = "string",
                                    FieldDescription = "AWS Secret Key",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = true
                                },
                                new()
                                {
                                    FieldName = "Region",
                                    FieldType = "string",
                                    FieldDescription = "AWS Region",
                                    IsRequired = true,
                                    DefaultValue = "ap-northeast-2",
                                    IsSecret = false
                                }
                            }
                        },
                        new()
                        {
                            Id = "azure-static",
                            Name = "Azure Static Credential Provider Plugin",
                            Description = "Azure ClientID/Client Secret을 사용하는 Credential Provider Plugin",
                            SamplePluginConfiguration = JsonSerializer.SerializeToDocument(new AzureStaticProviderConfig
                            {
                                ClientId = "Client ID",
                                ClientSecret = "Client Secret",
                                SubscriptionId = "Subscription ID",
                                TenantId = "Tenant ID",
                                Region = "Seoul"
                            }),
                            CreatedAt = DateTimeOffset.Parse("2024-02-25T00:00:00Z"),
                            UpdatedAt = DateTimeOffset.Parse("2024-02-25T00:00:00Z"),
                            PluginTypeDefinitions = new List<PluginTypeDefinition>
                            {
                                new()
                                {
                                    FieldName = "ClientId",
                                    FieldType = "string",
                                    FieldDescription = "Azure Client ID",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = false
                                },
                                new()
                                {
                                    FieldName = "ClientSecret",
                                    FieldType = "string",
                                    FieldDescription = "Azure Client Secret",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = true
                                },
                                new()
                                {
                                    FieldName = "SubscriptionId",
                                    FieldType = "string",
                                    FieldDescription = "Azure Subscription ID",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = false
                                },
                                new()
                                {
                                    FieldName = "TenantId",
                                    FieldType = "string",
                                    FieldDescription = "Azure Tenant ID",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = false
                                },
                                new()
                                {
                                    FieldName = "Region",
                                    FieldType = "string",
                                    FieldDescription = "Region",
                                    IsRequired = true,
                                    DefaultValue = "",
                                    IsSecret = false
                                }
                            }
                        }
                    });

        modelBuilder.Entity<PluginInstallation>()
                    .HasIndex(a => new
                    {
                        a.ChannelId,
                        a.PluginId
                    }).IsUnique();
        modelBuilder.Entity<PluginInstallation>()
                    .Property(a => a.PluginConfiguration)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<DeploymentLog>()
                    .Property(a => a.DeploymentStatus)
                    .HasConversion<string>();
        modelBuilder.Entity<DeploymentLog>()
                    .Property(a => a.DeploymentOutput)
                    .HasColumnType("jsonb");
        modelBuilder.Entity<DeploymentLog>()
                    .Property(a => a.CapturedBlockData)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<Sketch>()
                    .Property(a => a.BlockSketch)
                    .HasColumnType("jsonb");

        modelBuilder.Entity<PricingInformation>()
                    .Property(a => a.MachineType)
                    .HasConversion<string>();

        modelBuilder.Entity<PriceInfoPerVendor>()
                    .HasKey(a => new
                    {
                        a.PricingInformationId,
                        a.Vendor
                    });
        modelBuilder.Entity<PriceInfoPerVendor>()
                    .Property(a => a.Vendor)
                    .HasConversion<string>();

        modelBuilder.Entity<PricingInformation>()
                    .HasData(new List<PricingInformation>
                    {
                        // Virtual Machine Area
                        new()
                        {
                            Id = "01HRHEMYZQ5WM2V0Q154QD0KVZ",
                            MachineType = PricingMachineType.VirtualMachine,
                            Tier = "low"
                        },
                        new()
                        {
                            Id = "01HRHEQHSDHBDGECX87C71GXDN",
                            MachineType = PricingMachineType.VirtualMachine,
                            Tier = "medium"
                        },
                        new()
                        {
                            Id = "01HRHEWE71FXN562K3F3QEX6DT",
                            MachineType = PricingMachineType.VirtualMachine,
                            Tier = "large"
                        },

                        // Database Area
                        new()
                        {
                            Id = "01HRHEYBTNKH55TS6Z2ZE537VA",
                            MachineType = PricingMachineType.DatabaseServer,
                            Tier = "low"
                        },
                        new()
                        {
                            Id = "01HRHEYFN6DQG3R5CNYCRR3119",
                            MachineType = PricingMachineType.DatabaseServer,
                            Tier = "medium"
                        },
                        new()
                        {
                            Id = "01HRHEYK5AGZJF1TB21PZBA71W",
                            MachineType = PricingMachineType.DatabaseServer,
                            Tier = "large"
                        },

                        // Web Server Area
                        new()
                        {
                            Id = "01HRHF445XG2CJHDWDYQAT1BPK",
                            MachineType = PricingMachineType.WebServer,
                            Tier = "low"
                        },
                        new()
                        {
                            Id = "01HRHF48DY1V8FQNG5X769XPTQ",
                            MachineType = PricingMachineType.WebServer,
                            Tier = "medium"
                        },
                        new()
                        {
                            Id = "01HRHF4C4HDVZ19KYRMC11BXEX",
                            MachineType = PricingMachineType.WebServer,
                            Tier = "large"
                        }
                    });

        modelBuilder.Entity<PriceInfoPerVendor>()
                    .HasData(new List<PriceInfoPerVendor>
                    {
                        new()
                        {
                            PricingInformationId = "01HRHEMYZQ5WM2V0Q154QD0KVZ",
                            Vendor = VendorType.AWS,
                            TierInformation = "t2.medium",
                            PricePerHour = 0.0576m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHEQHSDHBDGECX87C71GXDN",
                            Vendor = VendorType.AWS,
                            TierInformation = "t2.xlarge",
                            PricePerHour = 0.2304m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHEWE71FXN562K3F3QEX6DT",
                            Vendor = VendorType.AWS,
                            TierInformation = "r6i.2xlarge",
                            PricePerHour = 0.5040m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHEYBTNKH55TS6Z2ZE537VA",
                            Vendor = VendorType.AWS,
                            TierInformation = "t4g.medium",
                            PricePerHour = 0.1020m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHEYFN6DQG3R5CNYCRR3119",
                            Vendor = VendorType.AWS,
                            TierInformation = "t4g.large",
                            PricePerHour = 0.2030m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHEYK5AGZJF1TB21PZBA71W",
                            Vendor = VendorType.AWS,
                            TierInformation = "r6g.xlarge",
                            PricePerHour = 0.5400m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHF445XG2CJHDWDYQAT1BPK",
                            Vendor = VendorType.AWS,
                            TierInformation = "t2.medium",
                            PricePerHour = 0.0576m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHF48DY1V8FQNG5X769XPTQ",
                            Vendor = VendorType.AWS,
                            TierInformation = "t2.xlarge",
                            PricePerHour = 0.2304m
                        },
                        new()
                        {
                            PricingInformationId = "01HRHF4C4HDVZ19KYRMC11BXEX",
                            Vendor = VendorType.AWS,
                            TierInformation = "r6i.2xlarge",
                            PricePerHour = 0.5040m
                        }
                    });

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
﻿// <auto-generated />
using System;
using System.Text.Json;
using BlockInfrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240310031136_AddCapturedSketchOnDeploymentLog")]
    partial class AddCapturedSketchOnDeploymentLog
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.BlockFile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<byte[]>("BinaryData")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("ContentDispositionFileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentDispositionName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BlockFiles");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Channel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PluginId")
                        .HasColumnType("text");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.ChannelPermission", b =>
                {
                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("ChannelPermissionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ChannelId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ChannelPermissions");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Credential", b =>
                {
                    b.Property<string>("CredentialId")
                        .HasColumnType("text");

                    b.Property<string>("CredentialProvider")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CredentialKey")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CredentialId", "CredentialProvider");

                    b.HasIndex("UserId");

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.DeploymentLog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<JsonDocument>("CapturedBlockData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<JsonDocument>("DeploymentOutput")
                        .HasColumnType("jsonb");

                    b.Property<string>("DeploymentStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PluginInstallationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SketchId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("PluginInstallationId");

                    b.HasIndex("SketchId");

                    b.ToTable("DeploymentLogs");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Plugin", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PluginTypeDefinitions")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<JsonDocument>("SamplePluginConfiguration")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Plugins");

                    b.HasData(
                        new
                        {
                            Id = "aws-static",
                            CreatedAt = new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Description = "Amazon Access Key를 사용하는 Credential Provider Plugin",
                            Name = "Amazon Static Credential Provider Plugin",
                            PluginTypeDefinitions = "[{\"FieldName\":\"AccessKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Access Key\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"SecretKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Secret Key\",\"IsRequired\":true,\"IsSecret\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"Region\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Region\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"ap-northeast-2\"}]",
                            SamplePluginConfiguration = System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()),
                            UpdatedAt = new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0))
                        });
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PluginInstallation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<JsonDocument>("PluginConfiguration")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("PluginId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.HasIndex("ChannelId", "PluginId")
                        .IsUnique();

                    b.ToTable("PluginInstallations");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PriceInfoPerVendor", b =>
                {
                    b.Property<string>("PricingInformationId")
                        .HasColumnType("text");

                    b.Property<string>("Vendor")
                        .HasColumnType("text");

                    b.Property<decimal>("PricePerHour")
                        .HasColumnType("numeric");

                    b.Property<string>("TierInformation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PricingInformationId", "Vendor");

                    b.ToTable("PriceInfoPerVendors");

                    b.HasData(
                        new
                        {
                            PricingInformationId = "01HRHEMYZQ5WM2V0Q154QD0KVZ",
                            Vendor = "AWS",
                            PricePerHour = 0.0576m,
                            TierInformation = "t2.medium"
                        },
                        new
                        {
                            PricingInformationId = "01HRHEQHSDHBDGECX87C71GXDN",
                            Vendor = "AWS",
                            PricePerHour = 0.2304m,
                            TierInformation = "t2.xlarge"
                        },
                        new
                        {
                            PricingInformationId = "01HRHEWE71FXN562K3F3QEX6DT",
                            Vendor = "AWS",
                            PricePerHour = 0.5040m,
                            TierInformation = "r6i.2xlarge"
                        },
                        new
                        {
                            PricingInformationId = "01HRHEYBTNKH55TS6Z2ZE537VA",
                            Vendor = "AWS",
                            PricePerHour = 0.1020m,
                            TierInformation = "t4g.medium"
                        },
                        new
                        {
                            PricingInformationId = "01HRHEYFN6DQG3R5CNYCRR3119",
                            Vendor = "AWS",
                            PricePerHour = 0.2030m,
                            TierInformation = "t4g.large"
                        },
                        new
                        {
                            PricingInformationId = "01HRHEYK5AGZJF1TB21PZBA71W",
                            Vendor = "AWS",
                            PricePerHour = 0.5400m,
                            TierInformation = "r6g.xlarge"
                        },
                        new
                        {
                            PricingInformationId = "01HRHF445XG2CJHDWDYQAT1BPK",
                            Vendor = "AWS",
                            PricePerHour = 0.0576m,
                            TierInformation = "t2.medium"
                        },
                        new
                        {
                            PricingInformationId = "01HRHF48DY1V8FQNG5X769XPTQ",
                            Vendor = "AWS",
                            PricePerHour = 0.2304m,
                            TierInformation = "t2.xlarge"
                        },
                        new
                        {
                            PricingInformationId = "01HRHF4C4HDVZ19KYRMC11BXEX",
                            Vendor = "AWS",
                            PricePerHour = 0.5040m,
                            TierInformation = "r6i.2xlarge"
                        });
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PricingInformation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("MachineType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PricingInformations");

                    b.HasData(
                        new
                        {
                            Id = "01HRHEMYZQ5WM2V0Q154QD0KVZ",
                            MachineType = "VirtualMachine",
                            Tier = "low"
                        },
                        new
                        {
                            Id = "01HRHEQHSDHBDGECX87C71GXDN",
                            MachineType = "VirtualMachine",
                            Tier = "medium"
                        },
                        new
                        {
                            Id = "01HRHEWE71FXN562K3F3QEX6DT",
                            MachineType = "VirtualMachine",
                            Tier = "large"
                        },
                        new
                        {
                            Id = "01HRHEYBTNKH55TS6Z2ZE537VA",
                            MachineType = "DatabaseServer",
                            Tier = "low"
                        },
                        new
                        {
                            Id = "01HRHEYFN6DQG3R5CNYCRR3119",
                            MachineType = "DatabaseServer",
                            Tier = "medium"
                        },
                        new
                        {
                            Id = "01HRHEYK5AGZJF1TB21PZBA71W",
                            MachineType = "DatabaseServer",
                            Tier = "large"
                        },
                        new
                        {
                            Id = "01HRHF445XG2CJHDWDYQAT1BPK",
                            MachineType = "WebServer",
                            Tier = "low"
                        },
                        new
                        {
                            Id = "01HRHF48DY1V8FQNG5X769XPTQ",
                            MachineType = "WebServer",
                            Tier = "medium"
                        },
                        new
                        {
                            Id = "01HRHF4C4HDVZ19KYRMC11BXEX",
                            MachineType = "WebServer",
                            Tier = "large"
                        });
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.RefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Sketch", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<JsonDocument>("BlockSketch")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Sketches");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfilePictureImageUrl")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Channel", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.Plugin", null)
                        .WithMany("ChannelList")
                        .HasForeignKey("PluginId");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.ChannelPermission", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.Channel", "Channel")
                        .WithMany("ChannelPermissionList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Common.Models.Data.User", "User")
                        .WithMany("ChannelPermissionList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Credential", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.User", "User")
                        .WithMany("CredentialList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.DeploymentLog", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Common.Models.Data.PluginInstallation", "PluginInstallation")
                        .WithMany()
                        .HasForeignKey("PluginInstallationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Common.Models.Data.Sketch", "Sketch")
                        .WithMany()
                        .HasForeignKey("SketchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("PluginInstallation");

                    b.Navigation("Sketch");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PluginInstallation", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.Channel", "Channel")
                        .WithMany("PluginInstallationList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Common.Models.Data.Plugin", "Plugin")
                        .WithMany("PluginInstallations")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PriceInfoPerVendor", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.PricingInformation", "PricingInformation")
                        .WithMany("PriceInfoPerVendors")
                        .HasForeignKey("PricingInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PricingInformation");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.RefreshToken", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Sketch", b =>
                {
                    b.HasOne("BlockInfrastructure.Common.Models.Data.Channel", "Channel")
                        .WithMany("SketchList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Channel", b =>
                {
                    b.Navigation("ChannelPermissionList");

                    b.Navigation("PluginInstallationList");

                    b.Navigation("SketchList");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.Plugin", b =>
                {
                    b.Navigation("ChannelList");

                    b.Navigation("PluginInstallations");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.PricingInformation", b =>
                {
                    b.Navigation("PriceInfoPerVendors");
                });

            modelBuilder.Entity("BlockInfrastructure.Common.Models.Data.User", b =>
                {
                    b.Navigation("ChannelPermissionList");

                    b.Navigation("CredentialList");
                });
#pragma warning restore 612, 618
        }
    }
}

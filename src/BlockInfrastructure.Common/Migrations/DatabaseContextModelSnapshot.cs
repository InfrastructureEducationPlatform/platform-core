﻿// <auto-generated />
using System;
using System.Text.Json;
using BlockInfrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Channel", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.ChannelPermission", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Credential", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.DeploymentLog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<JsonDocument>("DeploymentOutput")
                        .HasColumnType("jsonb");

                    b.Property<string>("DeploymentStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PluginId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SketchId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.HasIndex("SketchId");

                    b.ToTable("DeploymentLogs");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Plugin", b =>
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

                    b.Property<JsonDocument>("SamplePluginConfiguration")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.PluginInstallation", b =>
                {
                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<string>("PluginId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<JsonDocument>("PluginConfiguration")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ChannelId", "PluginId");

                    b.HasIndex("PluginId");

                    b.ToTable("PluginInstallations");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.RefreshToken", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Sketch", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.User", b =>
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

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Channel", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.Plugin", null)
                        .WithMany("ChannelList")
                        .HasForeignKey("PluginId");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.ChannelPermission", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.Channel", "Channel")
                        .WithMany("ChannelPermissionList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Core.Models.Data.User", "User")
                        .WithMany("ChannelPermissionList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Credential", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.User", "User")
                        .WithMany("CredentialList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.DeploymentLog", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.Plugin", "Plugin")
                        .WithMany()
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Core.Models.Data.Sketch", "Sketch")
                        .WithMany()
                        .HasForeignKey("SketchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plugin");

                    b.Navigation("Sketch");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.PluginInstallation", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.Channel", "Channel")
                        .WithMany("PluginInstallationList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BlockInfrastructure.Core.Models.Data.Plugin", "Plugin")
                        .WithMany()
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("Plugin");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.RefreshToken", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Sketch", b =>
                {
                    b.HasOne("BlockInfrastructure.Core.Models.Data.Channel", "Channel")
                        .WithMany("SketchList")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Channel", b =>
                {
                    b.Navigation("ChannelPermissionList");

                    b.Navigation("PluginInstallationList");

                    b.Navigation("SketchList");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.Plugin", b =>
                {
                    b.Navigation("ChannelList");
                });

            modelBuilder.Entity("BlockInfrastructure.Core.Models.Data.User", b =>
                {
                    b.Navigation("ChannelPermissionList");

                    b.Navigation("CredentialList");
                });
#pragma warning restore 612, 618
        }
    }
}

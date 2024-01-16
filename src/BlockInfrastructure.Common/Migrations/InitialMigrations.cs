#nullable disable

using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockInfrastructure.Common.Migrations;

/// <inheritdoc />
public partial class InitialMigrations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Plugins",
            table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Description = table.Column<string>("text", nullable: false),
                SamplePluginConfiguration = table.Column<JsonDocument>("jsonb", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Plugins", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Users",
            table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Email = table.Column<string>("text", nullable: false),
                ProfilePictureImageUrl = table.Column<string>("text", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            "Channels",
            table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Description = table.Column<string>("text", nullable: false),
                ProfileImageUrl = table.Column<string>("text", nullable: true),
                PluginId = table.Column<string>("text", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Channels", x => x.Id);
                table.ForeignKey(
                    "FK_Channels_Plugins_PluginId",
                    x => x.PluginId,
                    "Plugins",
                    "Id");
            });

        migrationBuilder.CreateTable(
            "Credentials",
            table => new
            {
                CredentialId = table.Column<string>("text", nullable: false),
                CredentialProvider = table.Column<string>("text", nullable: false),
                CredentialKey = table.Column<string>("text", nullable: true),
                UserId = table.Column<string>("text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Credentials", x => new
                {
                    x.CredentialId,
                    x.CredentialProvider
                });
                table.ForeignKey(
                    "FK_Credentials_Users_UserId",
                    x => x.UserId,
                    "Users",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "ChannelPermissions",
            table => new
            {
                UserId = table.Column<string>("text", nullable: false),
                ChannelId = table.Column<string>("text", nullable: false),
                ChannelPermissionType = table.Column<string>("text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChannelPermissions", x => new
                {
                    x.ChannelId,
                    x.UserId
                });
                table.ForeignKey(
                    "FK_ChannelPermissions_Channels_ChannelId",
                    x => x.ChannelId,
                    "Channels",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_ChannelPermissions_Users_UserId",
                    x => x.UserId,
                    "Users",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "PluginInstallations",
            table => new
            {
                ChannelId = table.Column<string>("text", nullable: false),
                PluginId = table.Column<string>("text", nullable: false),
                PluginConfiguration = table.Column<JsonDocument>("jsonb", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PluginInstallations", x => new
                {
                    x.ChannelId,
                    x.PluginId
                });
                table.ForeignKey(
                    "FK_PluginInstallations_Channels_ChannelId",
                    x => x.ChannelId,
                    "Channels",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_PluginInstallations_Plugins_PluginId",
                    x => x.PluginId,
                    "Plugins",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Sketches",
            table => new
            {
                Id = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("text", nullable: false),
                Description = table.Column<string>("text", nullable: false),
                ChannelId = table.Column<string>("text", nullable: false),
                BlockSketch = table.Column<JsonDocument>("jsonb", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sketches", x => x.Id);
                table.ForeignKey(
                    "FK_Sketches_Channels_ChannelId",
                    x => x.ChannelId,
                    "Channels",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "DeploymentLogs",
            table => new
            {
                Id = table.Column<string>("text", nullable: false),
                SketchId = table.Column<string>("text", nullable: false),
                PluginId = table.Column<string>("text", nullable: false),
                DeploymentStatus = table.Column<string>("text", nullable: false),
                DeploymentOutput = table.Column<JsonDocument>("jsonb", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>("timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeploymentLogs", x => x.Id);
                table.ForeignKey(
                    "FK_DeploymentLogs_Plugins_PluginId",
                    x => x.PluginId,
                    "Plugins",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_DeploymentLogs_Sketches_SketchId",
                    x => x.SketchId,
                    "Sketches",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_ChannelPermissions_UserId",
            "ChannelPermissions",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_Channels_PluginId",
            "Channels",
            "PluginId");

        migrationBuilder.CreateIndex(
            "IX_Credentials_UserId",
            "Credentials",
            "UserId");

        migrationBuilder.CreateIndex(
            "IX_DeploymentLogs_PluginId",
            "DeploymentLogs",
            "PluginId");

        migrationBuilder.CreateIndex(
            "IX_DeploymentLogs_SketchId",
            "DeploymentLogs",
            "SketchId");

        migrationBuilder.CreateIndex(
            "IX_PluginInstallations_PluginId",
            "PluginInstallations",
            "PluginId");

        migrationBuilder.CreateIndex(
            "IX_Sketches_ChannelId",
            "Sketches",
            "ChannelId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ChannelPermissions");

        migrationBuilder.DropTable(
            "Credentials");

        migrationBuilder.DropTable(
            "DeploymentLogs");

        migrationBuilder.DropTable(
            "PluginInstallations");

        migrationBuilder.DropTable(
            "Users");

        migrationBuilder.DropTable(
            "Sketches");

        migrationBuilder.DropTable(
            "Channels");

        migrationBuilder.DropTable(
            "Plugins");
    }
}
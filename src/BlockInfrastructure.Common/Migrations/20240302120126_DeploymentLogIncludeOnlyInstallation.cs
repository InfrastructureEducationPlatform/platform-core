using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class DeploymentLogIncludeOnlyInstallation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentLogs_Plugins_PluginId",
                table: "DeploymentLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PluginInstallations",
                table: "PluginInstallations");

            migrationBuilder.RenameColumn(
                name: "PluginId",
                table: "DeploymentLogs",
                newName: "PluginInstallationId");

            migrationBuilder.RenameIndex(
                name: "IX_DeploymentLogs_PluginId",
                table: "DeploymentLogs",
                newName: "IX_DeploymentLogs_PluginInstallationId");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PluginInstallations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PluginInstallations",
                table: "PluginInstallations",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.CreateIndex(
                name: "IX_PluginInstallations_ChannelId_PluginId",
                table: "PluginInstallations",
                columns: new[] { "ChannelId", "PluginId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentLogs_PluginInstallations_PluginInstallationId",
                table: "DeploymentLogs",
                column: "PluginInstallationId",
                principalTable: "PluginInstallations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentLogs_PluginInstallations_PluginInstallationId",
                table: "DeploymentLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PluginInstallations",
                table: "PluginInstallations");

            migrationBuilder.DropIndex(
                name: "IX_PluginInstallations_ChannelId_PluginId",
                table: "PluginInstallations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PluginInstallations");

            migrationBuilder.RenameColumn(
                name: "PluginInstallationId",
                table: "DeploymentLogs",
                newName: "PluginId");

            migrationBuilder.RenameIndex(
                name: "IX_DeploymentLogs_PluginInstallationId",
                table: "DeploymentLogs",
                newName: "IX_DeploymentLogs_PluginId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PluginInstallations",
                table: "PluginInstallations",
                columns: new[] { "ChannelId", "PluginId" });

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentLogs_Plugins_PluginId",
                table: "DeploymentLogs",
                column: "PluginId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

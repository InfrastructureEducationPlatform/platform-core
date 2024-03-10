using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddCapturedSketchOnDeploymentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<JsonDocument>(
                name: "CapturedBlockData",
                table: "DeploymentLogs",
                type: "jsonb",
                nullable: false);

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapturedBlockData",
                table: "DeploymentLogs");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));
        }
    }
}

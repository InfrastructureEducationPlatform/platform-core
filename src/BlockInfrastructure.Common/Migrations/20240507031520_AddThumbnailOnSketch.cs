using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailOnSketch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImageUrl",
                table: "Sketches",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "azure-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"SubscriptionId\":\"Subscription ID\",\"ClientId\":\"Client ID\",\"ClientSecret\":\"Client Secret\",\"TenantId\":\"Tenant ID\",\"Region\":\"Seoul\"}", new System.Text.Json.JsonDocumentOptions()));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailImageUrl",
                table: "Sketches");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "azure-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"SubscriptionId\":\"Subscription ID\",\"ClientId\":\"Client ID\",\"ClientSecret\":\"Client Secret\",\"TenantId\":\"Tenant ID\",\"Region\":\"Seoul\"}", new System.Text.Json.JsonDocumentOptions()));
        }
    }
}

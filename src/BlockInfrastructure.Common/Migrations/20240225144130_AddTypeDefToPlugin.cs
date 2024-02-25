using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeDefToPlugin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PluginTypeDefinitions",
                table: "Plugins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                columns: new[] { "CreatedAt", "PluginTypeDefinitions", "SamplePluginConfiguration", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[{\"FieldName\":\"AccessKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Access Key\",\"IsRequired\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"SecretKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Secret Key\",\"IsRequired\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"Region\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Region\",\"IsRequired\":true,\"DefaultValue\":\"ap-northeast-2\"}]", System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()), new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PluginTypeDefinitions",
                table: "Plugins");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                columns: new[] { "CreatedAt", "SamplePluginConfiguration", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}

using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddAwsPluginDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Plugins",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "SamplePluginConfiguration", "UpdatedAt" },
                values: new object[] { "aws-static", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Amazon Access Key를 사용하는 Credential Provider Plugin", "Amazon Static Credential Provider Plugin", System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static");
        }
    }
}

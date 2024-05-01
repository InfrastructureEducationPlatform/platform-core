using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class SeedAzureStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.InsertData(
                table: "Plugins",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "PluginTypeDefinitions", "SamplePluginConfiguration", "UpdatedAt" },
                values: new object[] { "azure-static", new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Azure ClientID/Client Secret을 사용하는 Credential Provider Plugin", "Azure Static Credential Provider Plugin", "[{\"FieldName\":\"ClientId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"ClientSecret\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client Secret\",\"IsRequired\":true,\"IsSecret\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"SubscriptionId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Subscription ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"TenantId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Tenant ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"}]", System.Text.Json.JsonDocument.Parse("{\"SubscriptionId\":\"Subscription ID\",\"ClientId\":\"Client ID\",\"ClientSecret\":\"Client Secret\",\"TenantId\":\"Tenant ID\"}", new System.Text.Json.JsonDocumentOptions()), new DateTimeOffset(new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "azure-static");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));
        }
    }
}

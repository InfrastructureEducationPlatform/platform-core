using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddRegion : Migration
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

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "azure-static",
                columns: new[] { "PluginTypeDefinitions", "SamplePluginConfiguration" },
                values: new object[] { "[{\"FieldName\":\"ClientId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"ClientSecret\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client Secret\",\"IsRequired\":true,\"IsSecret\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"SubscriptionId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Subscription ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"TenantId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Tenant ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"Region\",\"FieldType\":\"string\",\"FieldDescription\":\"Region\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"}]", System.Text.Json.JsonDocument.Parse("{\"SubscriptionId\":\"Subscription ID\",\"ClientId\":\"Client ID\",\"ClientSecret\":\"Client Secret\",\"TenantId\":\"Tenant ID\",\"Region\":\"Seoul\"}", new System.Text.Json.JsonDocumentOptions()) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "PluginTypeDefinitions", "SamplePluginConfiguration" },
                values: new object[] { "[{\"FieldName\":\"ClientId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"ClientSecret\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Client Secret\",\"IsRequired\":true,\"IsSecret\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"SubscriptionId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Subscription ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"TenantId\",\"FieldType\":\"string\",\"FieldDescription\":\"Azure Tenant ID\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"}]", System.Text.Json.JsonDocument.Parse("{\"SubscriptionId\":\"Subscription ID\",\"ClientId\":\"Client ID\",\"ClientSecret\":\"Client Secret\",\"TenantId\":\"Tenant ID\"}", new System.Text.Json.JsonDocumentOptions()) });
        }
    }
}

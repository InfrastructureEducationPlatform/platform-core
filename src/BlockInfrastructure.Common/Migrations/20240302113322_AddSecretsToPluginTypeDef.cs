using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretsToPluginTypeDef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                columns: new[] { "PluginTypeDefinitions", "SamplePluginConfiguration" },
                values: new object[] { "[{\"FieldName\":\"AccessKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Access Key\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"\"},{\"FieldName\":\"SecretKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Secret Key\",\"IsRequired\":true,\"IsSecret\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"Region\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Region\",\"IsRequired\":true,\"IsSecret\":false,\"DefaultValue\":\"ap-northeast-2\"}]", System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                columns: new[] { "PluginTypeDefinitions", "SamplePluginConfiguration" },
                values: new object[] { "[{\"FieldName\":\"AccessKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Access Key\",\"IsRequired\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"SecretKey\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Secret Key\",\"IsRequired\":true,\"DefaultValue\":\"\"},{\"FieldName\":\"Region\",\"FieldType\":\"string\",\"FieldDescription\":\"AWS Region\",\"IsRequired\":true,\"DefaultValue\":\"ap-northeast-2\"}]", System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()) });
        }
    }
}

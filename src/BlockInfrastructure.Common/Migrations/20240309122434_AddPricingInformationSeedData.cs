using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingInformationSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TierInformation",
                table: "PriceInfoPerVendors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));

            migrationBuilder.InsertData(
                table: "PricingInformations",
                columns: new[] { "Id", "MachineType", "Tier" },
                values: new object[,]
                {
                    { "01HRHEMYZQ5WM2V0Q154QD0KVZ", "VirtualMachine", "low" },
                    { "01HRHEQHSDHBDGECX87C71GXDN", "VirtualMachine", "medium" },
                    { "01HRHEWE71FXN562K3F3QEX6DT", "VirtualMachine", "large" },
                    { "01HRHEYBTNKH55TS6Z2ZE537VA", "DatabaseServer", "low" },
                    { "01HRHEYFN6DQG3R5CNYCRR3119", "DatabaseServer", "medium" },
                    { "01HRHEYK5AGZJF1TB21PZBA71W", "DatabaseServer", "large" },
                    { "01HRHF445XG2CJHDWDYQAT1BPK", "WebServer", "low" },
                    { "01HRHF48DY1V8FQNG5X769XPTQ", "WebServer", "medium" },
                    { "01HRHF4C4HDVZ19KYRMC11BXEX", "WebServer", "large" }
                });

            migrationBuilder.InsertData(
                table: "PriceInfoPerVendors",
                columns: new[] { "PricingInformationId", "Vendor", "PricePerHour", "TierInformation" },
                values: new object[,]
                {
                    { "01HRHEMYZQ5WM2V0Q154QD0KVZ", "AWS", 0.0576m, "t2.medium" },
                    { "01HRHEQHSDHBDGECX87C71GXDN", "AWS", 0.2304m, "t2.xlarge" },
                    { "01HRHEWE71FXN562K3F3QEX6DT", "AWS", 0.5040m, "r6i.2xlarge" },
                    { "01HRHEYBTNKH55TS6Z2ZE537VA", "AWS", 0.1020m, "t4g.medium" },
                    { "01HRHEYFN6DQG3R5CNYCRR3119", "AWS", 0.2030m, "t4g.large" },
                    { "01HRHEYK5AGZJF1TB21PZBA71W", "AWS", 0.5400m, "r6g.xlarge" },
                    { "01HRHF445XG2CJHDWDYQAT1BPK", "AWS", 0.0576m, "t2.medium" },
                    { "01HRHF48DY1V8FQNG5X769XPTQ", "AWS", 0.2304m, "t2.xlarge" },
                    { "01HRHF4C4HDVZ19KYRMC11BXEX", "AWS", 0.5040m, "r6i.2xlarge" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEMYZQ5WM2V0Q154QD0KVZ", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEQHSDHBDGECX87C71GXDN", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEWE71FXN562K3F3QEX6DT", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEYBTNKH55TS6Z2ZE537VA", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEYFN6DQG3R5CNYCRR3119", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHEYK5AGZJF1TB21PZBA71W", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHF445XG2CJHDWDYQAT1BPK", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHF48DY1V8FQNG5X769XPTQ", "AWS" });

            migrationBuilder.DeleteData(
                table: "PriceInfoPerVendors",
                keyColumns: new[] { "PricingInformationId", "Vendor" },
                keyValues: new object[] { "01HRHF4C4HDVZ19KYRMC11BXEX", "AWS" });

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEMYZQ5WM2V0Q154QD0KVZ");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEQHSDHBDGECX87C71GXDN");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEWE71FXN562K3F3QEX6DT");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEYBTNKH55TS6Z2ZE537VA");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEYFN6DQG3R5CNYCRR3119");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHEYK5AGZJF1TB21PZBA71W");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHF445XG2CJHDWDYQAT1BPK");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHF48DY1V8FQNG5X769XPTQ");

            migrationBuilder.DeleteData(
                table: "PricingInformations",
                keyColumn: "Id",
                keyValue: "01HRHF4C4HDVZ19KYRMC11BXEX");

            migrationBuilder.DropColumn(
                name: "TierInformation",
                table: "PriceInfoPerVendors");

            migrationBuilder.UpdateData(
                table: "Plugins",
                keyColumn: "Id",
                keyValue: "aws-static",
                column: "SamplePluginConfiguration",
                value: System.Text.Json.JsonDocument.Parse("{\"AccessKey\":\"Access Key ID\",\"SecretKey\":\"Access Secret Key\",\"Region\":\"Default Region Code(i.e: ap-northeast-2)\"}", new System.Text.Json.JsonDocumentOptions()));
        }
    }
}

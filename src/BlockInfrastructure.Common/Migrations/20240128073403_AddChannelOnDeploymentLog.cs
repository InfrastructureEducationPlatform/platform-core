using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockInfrastructure.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelOnDeploymentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "DeploymentLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentLogs_ChannelId",
                table: "DeploymentLogs",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeploymentLogs_Channels_ChannelId",
                table: "DeploymentLogs",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeploymentLogs_Channels_ChannelId",
                table: "DeploymentLogs");

            migrationBuilder.DropIndex(
                name: "IX_DeploymentLogs_ChannelId",
                table: "DeploymentLogs");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "DeploymentLogs");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarBord.Migrations
{
    /// <inheritdoc />
    public partial class AddTrustpilotOAuthFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternaalBussinessId",
                table: "PlatformTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "PlatformTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternaalBussinessId",
                table: "PlatformTokens");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "PlatformTokens");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarBord.Migrations
{
    /// <inheritdoc />
    public partial class RenameRiviewDateToReviewDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RiviewDate",
                table: "Reviews",
                newName: "ReviewDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewDate",
                table: "Reviews",
                newName: "RiviewDate");
        }
    }
}

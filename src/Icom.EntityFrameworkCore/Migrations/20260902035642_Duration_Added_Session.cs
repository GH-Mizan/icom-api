using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class Duration_Added_Session : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "BtebSessions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "BtebSessions");
        }
    }
}

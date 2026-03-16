using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class CourseCompleted_Added_Student : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CertificateDistributed",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CourseCompleted",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateDistributed",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CourseCompleted",
                table: "Students");
        }
    }
}

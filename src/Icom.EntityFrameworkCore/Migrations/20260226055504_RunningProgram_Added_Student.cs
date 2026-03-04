using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class RunningProgram_Added_Student : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSertificateProvided",
                table: "BtebSessions",
                newName: "IsCertificateProvided");

            migrationBuilder.AddColumn<int>(
                name: "RunningProgram",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunningProgram",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "IsCertificateProvided",
                table: "BtebSessions",
                newName: "IsSertificateProvided");
        }
    }
}

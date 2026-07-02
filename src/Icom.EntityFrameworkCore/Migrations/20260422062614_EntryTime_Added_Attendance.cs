using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class EntryTime_Added_Attendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BTEB",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "Attendances",
                newName: "EntryTime");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "EntryTime",
                table: "Attendances",
                newName: "StudentName");

            migrationBuilder.AddColumn<bool>(
                name: "BTEB",
                table: "Attendances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

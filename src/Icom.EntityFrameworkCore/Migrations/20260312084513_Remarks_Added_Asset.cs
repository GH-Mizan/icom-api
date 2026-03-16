using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class Remarks_Added_Asset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Assets");
        }
    }
}

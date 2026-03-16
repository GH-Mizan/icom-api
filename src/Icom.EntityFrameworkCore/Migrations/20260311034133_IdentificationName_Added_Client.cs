using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class IdentificationName_Added_Client : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentificationName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentificationName",
                table: "Clients");
        }
    }
}

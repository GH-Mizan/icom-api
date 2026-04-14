using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class Discount_Added_Service : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Services",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetServiceCharge",
                table: "Services",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NetServiceCharge",
                table: "Services");
        }
    }
}

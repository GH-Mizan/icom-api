using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Icom.Migrations
{
    /// <inheritdoc />
    public partial class ServiceType_Changed_InvoiceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTypes",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<string>(
                name: "ServiceTypes",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class removedTicketIDasitwasnotnesesarry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "Ticket",
                newName: "Seat");

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Ticket",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Ticket");

            migrationBuilder.RenameColumn(
                name: "Seat",
                table: "Ticket",
                newName: "TicketId");
        }
    }
}

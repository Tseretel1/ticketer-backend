using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptionToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ticket");
        }
    }
}

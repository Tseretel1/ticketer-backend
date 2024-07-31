using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class tikcetanduserrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PublisherID",
                table: "Tickets",
                column: "PublisherID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_User_PublisherID",
                table: "Tickets",
                column: "PublisherID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_User_PublisherID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PublisherID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PublisherID",
                table: "Tickets");
        }

    }
}

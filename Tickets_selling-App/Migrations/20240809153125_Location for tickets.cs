using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class Locationfortickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreatorValidation_User_Userid",
                table: "CreatorValidation");

            migrationBuilder.DropIndex(
                name: "IX_CreatorValidation_Userid",
                table: "CreatorValidation");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorValidation_Userid",
                table: "CreatorValidation",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorValidation_User_Userid",
                table: "CreatorValidation",
                column: "Userid",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

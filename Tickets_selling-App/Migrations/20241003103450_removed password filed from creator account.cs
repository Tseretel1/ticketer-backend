using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class removedpasswordfiledfromcreatoraccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "CreatorAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "CreatorAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

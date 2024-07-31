using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class ActiveornotforCreatorGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Creator_CreatorGroup_creatorGroupId",
                table: "Creator");

            migrationBuilder.DropForeignKey(
                name: "FK_Creator_User_UserID",
                table: "Creator");

            migrationBuilder.DropIndex(
                name: "IX_Creator_creatorGroupId",
                table: "Creator");

            migrationBuilder.DropIndex(
                name: "IX_Creator_UserID",
                table: "Creator");

            migrationBuilder.DropColumn(
                name: "creatorGroupId",
                table: "Creator");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "CreatorGroup",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "CreatorGroup");

            migrationBuilder.AddColumn<int>(
                name: "creatorGroupId",
                table: "Creator",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Creator_creatorGroupId",
                table: "Creator",
                column: "creatorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Creator_UserID",
                table: "Creator",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Creator_CreatorGroup_creatorGroupId",
                table: "Creator",
                column: "creatorGroupId",
                principalTable: "CreatorGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Creator_User_UserID",
                table: "Creator",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

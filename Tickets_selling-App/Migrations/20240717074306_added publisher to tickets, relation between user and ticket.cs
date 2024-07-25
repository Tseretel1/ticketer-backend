using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    public partial class addedpublishertoticketsrelationbetweenuserandticket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add the column as nullable
            migrationBuilder.AddColumn<int>(
                name: "Publisher",
                table: "Tickets",
                type: "int",
                nullable: true);

            // Step 2: Set a default valid Publisher value for existing records
            migrationBuilder.Sql("UPDATE Tickets SET Publisher = (SELECT TOP 1 ID FROM [User]) WHERE Publisher IS NULL");

            // Step 3: Alter the column to be non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "Publisher",
                table: "Tickets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Step 4: Create the index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Publisher",
                table: "Tickets",
                column: "Publisher");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_User_Publisher",
                table: "Tickets",
                column: "Publisher",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_User_Publisher",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Publisher",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Tickets");
        }
    }
}

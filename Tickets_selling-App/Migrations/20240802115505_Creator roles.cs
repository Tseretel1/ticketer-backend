using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickets_selling_App.Migrations
{
    /// <inheritdoc />
    public partial class Creatorroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorID = table.Column<int>(type: "int", nullable: false),
                    creatorGroupId = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_GroupRoles_CreatorGroup_creatorGroupId",
                        column: x => x.creatorGroupId,
                        principalTable: "CreatorGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRoles_Creator_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "Creator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_creatorGroupId",
                table: "GroupRoles",
                column: "creatorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoles_CreatorID",
                table: "GroupRoles",
                column: "CreatorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoles");
        }
    }
}


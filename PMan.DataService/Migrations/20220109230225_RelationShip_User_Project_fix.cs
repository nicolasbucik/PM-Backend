using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMan.DataService.Migrations
{
    public partial class RelationShip_User_Project_fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                table: "Projects",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UserId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId1",
                table: "Projects",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UserId1",
                table: "Projects",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

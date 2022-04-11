using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_2.Migrations
{
    public partial class MigrateDBs12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Tasks_AuthorId",
                table: "Solutions");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Tasks_TaskId",
                table: "Solutions",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Tasks_TaskId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_TaskId",
                table: "Solutions");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Tasks_AuthorId",
                table: "Solutions",
                column: "AuthorId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

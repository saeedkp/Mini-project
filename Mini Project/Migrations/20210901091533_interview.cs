using Microsoft.EntityFrameworkCore.Migrations;

namespace Mini_Project.Migrations
{
    public partial class interview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_interviews_RequestRefId",
                table: "interviews",
                column: "RequestRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_interviews_Requests_RequestRefId",
                table: "interviews",
                column: "RequestRefId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_interviews_Requests_RequestRefId",
                table: "interviews");

            migrationBuilder.DropIndex(
                name: "IX_interviews_RequestRefId",
                table: "interviews");
        }
    }
}

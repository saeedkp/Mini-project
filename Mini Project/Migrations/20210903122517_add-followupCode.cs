using Microsoft.EntityFrameworkCore.Migrations;

namespace Mini_Project.Migrations
{
    public partial class addfollowupCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "followUpCode",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "followUpCode",
                table: "Requests");
        }
    }
}

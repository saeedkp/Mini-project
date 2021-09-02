using Microsoft.EntityFrameworkCore.Migrations;

namespace Mini_Project.Migrations
{
    public partial class modifiedInterview_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "interviews",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "interviews");
        }
    }
}

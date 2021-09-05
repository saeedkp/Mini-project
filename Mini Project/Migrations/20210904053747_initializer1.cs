using Microsoft.EntityFrameworkCore.Migrations;

namespace Mini_Project.Migrations
{
    public partial class initializer1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "68587266-51ea-492e-88d5-f74e6639238c", "9b6dd4fa-0427-4135-a090-7dc110561788", "Admin", "ADMIN" },
                    { "65aa5753-f72d-4a18-9edd-acc3047b9b10", "7e83289c-a4c2-4403-9629-a276e473a5ac", "HRM", "HRM" },
                    { "a506ffb1-cf4e-4605-9bd3-56721ae93784", "8f31c10c-a8f1-4a8b-9ccd-10042840cba6", "Tech Lead", "TECH LEAD" },
                    { "b621cdcf-2ff4-461d-88b5-21e445c789f4", "24792f93-0ade-4c50-9ad0-57d90d293194", "Office Manager", "OFFICE MANAGER" },
                    { "f0e92155-1a16-4f39-9b73-1b744c32093c", "7b776949-6a9a-4b4a-ab08-56be91a3596d", "Trainee", "TRAINEE" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65aa5753-f72d-4a18-9edd-acc3047b9b10");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68587266-51ea-492e-88d5-f74e6639238c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a506ffb1-cf4e-4605-9bd3-56721ae93784");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b621cdcf-2ff4-461d-88b5-21e445c789f4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f0e92155-1a16-4f39-9b73-1b744c32093c");
        }
    }
}

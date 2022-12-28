using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringWebApplication.Migrations
{
    public partial class ThirdConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "sales",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "carts");
        }
    }
}

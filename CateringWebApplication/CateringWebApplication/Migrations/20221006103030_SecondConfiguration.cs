using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringWebApplication.Migrations
{
    public partial class SecondConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "productsSold",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_productsSold",
                table: "productsSold",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_productsSold",
                table: "productsSold");

            migrationBuilder.DropColumn(
                name: "id",
                table: "productsSold");
        }
    }
}

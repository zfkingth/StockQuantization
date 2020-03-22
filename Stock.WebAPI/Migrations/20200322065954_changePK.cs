using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class changePK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TempPrice",
                table: "TempPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceSet",
                table: "PriceSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TempPrice",
                table: "TempPrice",
                columns: new[] { "Code", "Unit", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceSet",
                table: "PriceSet",
                columns: new[] { "Code", "Unit", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TempPrice",
                table: "TempPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceSet",
                table: "PriceSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TempPrice",
                table: "TempPrice",
                columns: new[] { "Unit", "Code", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceSet",
                table: "PriceSet",
                columns: new[] { "Unit", "Code", "Date" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class modifyKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD",
                columns: new[] { "Code", "AXrDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD",
                column: "Code");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class add_key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD");

            migrationBuilder.AlterColumn<string>(
                name: "BonusType",
                table: "StockXRXD",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10) CHARACTER SET utf8mb4",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD",
                columns: new[] { "Code", "AXrDate", "BonusType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD");

            migrationBuilder.AlterColumn<string>(
                name: "BonusType",
                table: "StockXRXD",
                type: "varchar(10) CHARACTER SET utf8mb4",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockXRXD",
                table: "StockXRXD",
                columns: new[] { "Code", "AXrDate" });
        }
    }
}

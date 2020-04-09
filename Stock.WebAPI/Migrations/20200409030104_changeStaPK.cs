using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class changeStaPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaPrice",
                table: "StaPrice");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StaPrice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaPrice",
                table: "StaPrice",
                columns: new[] { "Unit", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StaPrice",
                table: "StaPrice");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StaPrice",
                type: "varchar(15) CHARACTER SET utf8mb4",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaPrice",
                table: "StaPrice",
                columns: new[] { "Code", "Unit", "Date" });
        }
    }
}

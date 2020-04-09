using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class addStaPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaPrice",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    HighlimitNum = table.Column<int>(nullable: false),
                    LowlimitNum = table.Column<int>(nullable: false),
                    FailNum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaPrice", x => new { x.Code, x.Unit, x.Date });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaPrice");
        }
    }
}

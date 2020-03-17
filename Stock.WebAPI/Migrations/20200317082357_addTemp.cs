using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class addTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempPrice",
                columns: table => new
                {
                    Unit = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<double>(nullable: false),
                    Close = table.Column<double>(nullable: false),
                    High = table.Column<double>(nullable: false),
                    Low = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    Money = table.Column<double>(nullable: false),
                    Paused = table.Column<bool>(nullable: false),
                    Highlimit = table.Column<double>(nullable: false),
                    Lowlimit = table.Column<double>(nullable: false),
                    Avg = table.Column<double>(nullable: false),
                    Preclose = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempPrice", x => new { x.Unit, x.Code, x.Date });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempPrice");
        }
    }
}

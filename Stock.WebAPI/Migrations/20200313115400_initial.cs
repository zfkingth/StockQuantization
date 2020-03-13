using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceSet",
                columns: table => new
                {
                    Unit = table.Column<byte>(nullable: false),
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
                    table.PrimaryKey("PK_PriceSet", x => new { x.Unit, x.Code, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "SecuritiesSet",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Displayname = table.Column<string>(maxLength: 10, nullable: true),
                    Name = table.Column<string>(maxLength: 10, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuritiesSet", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceSet");

            migrationBuilder.DropTable(
                name: "SecuritiesSet");
        }
    }
}

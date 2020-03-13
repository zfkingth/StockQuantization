using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class remove30m : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Price30m");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Price30m",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(15) CHARACTER SET utf8mb4", maxLength: 15, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Avg = table.Column<double>(type: "double", nullable: false),
                    Close = table.Column<double>(type: "double", nullable: false),
                    High = table.Column<double>(type: "double", nullable: false),
                    Highlimit = table.Column<double>(type: "double", nullable: false),
                    Low = table.Column<double>(type: "double", nullable: false),
                    Lowlimit = table.Column<double>(type: "double", nullable: false),
                    Money = table.Column<double>(type: "double", nullable: false),
                    Open = table.Column<double>(type: "double", nullable: false),
                    Paused = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Preclose = table.Column<double>(type: "double", nullable: false),
                    Volume = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price30m", x => new { x.Code, x.Date });
                });
        }
    }
}

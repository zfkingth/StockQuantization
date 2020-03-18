using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class addTradDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeDays",
                columns: table => new
                {
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeDays", x => x.Date);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeDays");
        }
    }
}

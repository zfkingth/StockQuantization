using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class changeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarginTotal",
                columns: table => new
                {
                    Date = table.Column<DateTime>(nullable: false),
                    ExchangeCode = table.Column<string>(maxLength: 12, nullable: false),
                    FinValue = table.Column<double>(nullable: false),
                    FinBuyValue = table.Column<double>(nullable: false),
                    SecVolume = table.Column<int>(nullable: false),
                    SecValue = table.Column<double>(nullable: false),
                    SecSellVolume = table.Column<int>(nullable: false),
                    FinSecValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginTotal", x => new { x.Date, x.ExchangeCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginTotal");
        }
    }
}

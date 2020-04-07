using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class addMarketDeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketDeal",
                columns: table => new
                {
                    Day = table.Column<DateTime>(nullable: false),
                    LinkId = table.Column<int>(nullable: false),
                    LinkName = table.Column<string>(maxLength: 16, nullable: true),
                    BuyAmount = table.Column<double>(nullable: false),
                    SellAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDeal", x => new { x.Day, x.LinkId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDeal");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class add_xr_xd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockXRXD",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    DividendRatio = table.Column<double>(nullable: false),
                    TransferRatio = table.Column<double>(nullable: false),
                    BonusRatioRmb = table.Column<double>(nullable: false),
                    AXrDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockXRXD", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockXRXD");
        }
    }
}

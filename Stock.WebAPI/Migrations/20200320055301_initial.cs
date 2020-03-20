using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MesTime = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MesTime);
                });

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
                name: "SearchResultSet",
                columns: table => new
                {
                    ActionName = table.Column<string>(maxLength: 32, nullable: false),
                    ActionParams = table.Column<string>(maxLength: 512, nullable: false),
                    ActionDate = table.Column<DateTime>(nullable: false),
                    ActionReslut = table.Column<string>(maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResultSet", x => new { x.ActionName, x.ActionParams, x.ActionDate });
                });

            migrationBuilder.CreateTable(
                name: "SecuritiesSet",
                columns: table => new
                {
                    Type = table.Column<byte>(nullable: false),
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    Displayname = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 10, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuritiesSet", x => new { x.Type, x.Code });
                });

            migrationBuilder.CreateTable(
                name: "StockEvents",
                columns: table => new
                {
                    EventName = table.Column<string>(maxLength: 30, nullable: false),
                    LastAriseStartDate = table.Column<DateTime>(nullable: false),
                    LastAriseEndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEvents", x => x.EventName);
                });

            migrationBuilder.CreateTable(
                name: "StockXRXD",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 15, nullable: false),
                    AXrDate = table.Column<DateTime>(nullable: false),
                    BonusType = table.Column<string>(maxLength: 10, nullable: false),
                    DividendRatio = table.Column<double>(nullable: false),
                    TransferRatio = table.Column<double>(nullable: false),
                    BonusRatioRmb = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockXRXD", x => new { x.Code, x.AXrDate, x.BonusType });
                });

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

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    Username = table.Column<string>(maxLength: 20, nullable: false),
                    RoleName = table.Column<string>(maxLength: 10, nullable: false, defaultValue: "user"),
                    ExpiredDate = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Email = table.Column<string>(maxLength: 60, nullable: false),
                    Password = table.Column<string>(maxLength: 90, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesSet_Code",
                table: "SecuritiesSet",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "PriceSet");

            migrationBuilder.DropTable(
                name: "SearchResultSet");

            migrationBuilder.DropTable(
                name: "SecuritiesSet");

            migrationBuilder.DropTable(
                name: "StockEvents");

            migrationBuilder.DropTable(
                name: "StockXRXD");

            migrationBuilder.DropTable(
                name: "TempPrice");

            migrationBuilder.DropTable(
                name: "TradeDays");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

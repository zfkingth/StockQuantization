using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyStock.WebAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayDataSet",
                columns: table => new
                {
                    StockId = table.Column<string>(maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<float>(nullable: false),
                    Low = table.Column<float>(nullable: false),
                    High = table.Column<float>(nullable: false),
                    Close = table.Column<float>(nullable: false),
                    Volume = table.Column<float>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    ZhangDieFu = table.Column<float>(nullable: true),
                    ZongShiZhi = table.Column<float>(nullable: true),
                    LiuTongShiZhi = table.Column<float>(nullable: true),
                    HuanShouLiu = table.Column<float>(nullable: true),
                    Permanent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayDataSet", x => new { x.StockId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "MarginTotal",
                columns: table => new
                {
                    Date = table.Column<DateTime>(nullable: false),
                    FinValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginTotal", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "MarketDeal",
                columns: table => new
                {
                    MarketType = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DRZJLR = table.Column<float>(nullable: false),
                    Permanent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDeal", x => new { x.MarketType, x.Date });
                });

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
                name: "RealTimeDataSet",
                columns: table => new
                {
                    StockId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(maxLength: 20, nullable: false),
                    StockName = table.Column<string>(maxLength: 10, nullable: true),
                    Open = table.Column<float>(nullable: false),
                    Low = table.Column<float>(nullable: false),
                    High = table.Column<float>(nullable: false),
                    Close = table.Column<float>(nullable: false),
                    Volume = table.Column<float>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    ZhangDieFu = table.Column<float>(nullable: true),
                    ZongShiZhi = table.Column<float>(nullable: true),
                    LiuTongShiZhi = table.Column<float>(nullable: true),
                    HuanShouLiu = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealTimeDataSet", x => new { x.StockId, x.Date });
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
                name: "SharingSet",
                columns: table => new
                {
                    StockId = table.Column<string>(maxLength: 10, nullable: false),
                    DateGongGao = table.Column<DateTime>(nullable: false),
                    DateChuXi = table.Column<DateTime>(nullable: true),
                    DateDengJi = table.Column<DateTime>(nullable: true),
                    SongGu = table.Column<float>(nullable: false),
                    ZhuanZeng = table.Column<float>(nullable: false),
                    PaiXi = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharingSet", x => new { x.StockId, x.DateGongGao });
                });

            migrationBuilder.CreateTable(
                name: "StaPrice",
                columns: table => new
                {
                    Date = table.Column<DateTime>(nullable: false),
                    HighlimitNum = table.Column<int>(nullable: false),
                    LowlimitNum = table.Column<int>(nullable: false),
                    FailNum = table.Column<int>(nullable: false),
                    Permanent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaPrice", x => x.Date);
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
                name: "StockNumSet",
                columns: table => new
                {
                    StockId = table.Column<string>(maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    All = table.Column<double>(nullable: false),
                    LiuTongA = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockNumSet", x => new { x.StockId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "StockSet",
                columns: table => new
                {
                    StockId = table.Column<string>(maxLength: 10, nullable: false),
                    StockName = table.Column<string>(maxLength: 10, nullable: true),
                    RealDataUpdated = table.Column<DateTime>(nullable: false),
                    StockType = table.Column<int>(nullable: false),
                    MarketStartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSet", x => x.StockId);
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
                name: "IX_DayDataSet_Date",
                table: "DayDataSet",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayDataSet");

            migrationBuilder.DropTable(
                name: "MarginTotal");

            migrationBuilder.DropTable(
                name: "MarketDeal");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "RealTimeDataSet");

            migrationBuilder.DropTable(
                name: "SearchResultSet");

            migrationBuilder.DropTable(
                name: "SharingSet");

            migrationBuilder.DropTable(
                name: "StaPrice");

            migrationBuilder.DropTable(
                name: "StockEvents");

            migrationBuilder.DropTable(
                name: "StockNumSet");

            migrationBuilder.DropTable(
                name: "StockSet");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

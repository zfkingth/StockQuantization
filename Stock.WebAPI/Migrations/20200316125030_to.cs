using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class to : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "SearchResultSet");

            migrationBuilder.DropTable(
                name: "StockEvents");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

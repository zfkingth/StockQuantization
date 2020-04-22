using Microsoft.EntityFrameworkCore.Migrations;

namespace MyStock.WebAPI.Migrations
{
    public partial class change_MarketDeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal");

            migrationBuilder.DropColumn(
                name: "BuyAmount",
                table: "MarketDeal");

            migrationBuilder.DropColumn(
                name: "SellAmount",
                table: "MarketDeal");

            migrationBuilder.AddColumn<int>(
                name: "MarketType",
                table: "MarketDeal",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "DRZJLR",
                table: "MarketDeal",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal",
                columns: new[] { "MarketType", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal");

            migrationBuilder.DropColumn(
                name: "MarketType",
                table: "MarketDeal");

            migrationBuilder.DropColumn(
                name: "DRZJLR",
                table: "MarketDeal");

            migrationBuilder.AddColumn<double>(
                name: "BuyAmount",
                table: "MarketDeal",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SellAmount",
                table: "MarketDeal",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal",
                column: "Date");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class changeMarketDealPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal",
                columns: new[] { "LinkId", "Day" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketDeal",
                table: "MarketDeal",
                columns: new[] { "Day", "LinkId" });
        }
    }
}

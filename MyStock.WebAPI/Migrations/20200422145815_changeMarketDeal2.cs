using Microsoft.EntityFrameworkCore.Migrations;

namespace MyStock.WebAPI.Migrations
{
    public partial class changeMarketDeal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Permanent",
                table: "MarketDeal",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permanent",
                table: "MarketDeal");
        }
    }
}

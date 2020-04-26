using Microsoft.EntityFrameworkCore.Migrations;

namespace MyStock.WebAPI.Migrations
{
    public partial class initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDT",
                table: "DayDataSet");

            migrationBuilder.DropColumn(
                name: "IsZT",
                table: "DayDataSet");

            migrationBuilder.DropColumn(
                name: "PreClose",
                table: "DayDataSet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDT",
                table: "DayDataSet",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsZT",
                table: "DayDataSet",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "PreClose",
                table: "DayDataSet",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }
    }
}

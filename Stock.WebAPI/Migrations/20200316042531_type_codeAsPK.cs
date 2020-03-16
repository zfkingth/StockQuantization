using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class type_codeAsPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SecuritiesSet",
                table: "SecuritiesSet");

            migrationBuilder.DropIndex(
                name: "IX_SecuritiesSet_Type",
                table: "SecuritiesSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecuritiesSet",
                table: "SecuritiesSet",
                columns: new[] { "Type", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesSet_Code",
                table: "SecuritiesSet",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SecuritiesSet",
                table: "SecuritiesSet");

            migrationBuilder.DropIndex(
                name: "IX_SecuritiesSet_Code",
                table: "SecuritiesSet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecuritiesSet",
                table: "SecuritiesSet",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_SecuritiesSet_Type",
                table: "SecuritiesSet",
                column: "Type");
        }
    }
}

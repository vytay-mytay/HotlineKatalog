using Microsoft.EntityFrameworkCore.Migrations;

namespace HotlineKatalog.DAL.Migrations
{
    public partial class AlterShopTags_GoodUrlTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoodUrlTag",
                table: "ShopTags",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoodUrlTag",
                table: "ShopTags");
        }
    }
}

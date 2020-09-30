using Microsoft.EntityFrameworkCore.Migrations;

namespace HotlineKatalog.DAL.Migrations
{
    public partial class Alter_ShopTags_PageItemTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PageItemTag",
                table: "ShopTags",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageItemTag",
                table: "ShopTags");
        }
    }
}

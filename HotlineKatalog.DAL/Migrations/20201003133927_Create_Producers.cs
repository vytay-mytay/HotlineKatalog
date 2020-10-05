using Microsoft.EntityFrameworkCore.Migrations;

namespace HotlineKatalog.DAL.Migrations
{
    public partial class Create_Producers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProducerId",
                table: "Goods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Producers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goods_ProducerId",
                table: "Goods",
                column: "ProducerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_Producers_ProducerId",
                table: "Goods",
                column: "ProducerId",
                principalTable: "Producers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_Producers_ProducerId",
                table: "Goods");

            migrationBuilder.DropTable(
                name: "Producers");

            migrationBuilder.DropIndex(
                name: "IX_Goods_ProducerId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "ProducerId",
                table: "Goods");
        }
    }
}

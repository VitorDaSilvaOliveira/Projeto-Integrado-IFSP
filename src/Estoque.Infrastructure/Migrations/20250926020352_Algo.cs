using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Algo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoSerie_ProdutoLote_ProdutoLoteLoteId",
                table: "ProdutoSerie");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoSerie_ProdutoLoteLoteId",
                table: "ProdutoSerie");

            migrationBuilder.DropColumn(
                name: "ProdutoLoteLoteId",
                table: "ProdutoSerie");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoSerie_LoteId",
                table: "ProdutoSerie",
                column: "LoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoSerie_ProdutoLote_LoteId",
                table: "ProdutoSerie",
                column: "LoteId",
                principalTable: "ProdutoLote",
                principalColumn: "LoteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutoSerie_ProdutoLote_LoteId",
                table: "ProdutoSerie");

            migrationBuilder.DropIndex(
                name: "IX_ProdutoSerie_LoteId",
                table: "ProdutoSerie");

            migrationBuilder.AddColumn<int>(
                name: "ProdutoLoteLoteId",
                table: "ProdutoSerie",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoSerie_ProdutoLoteLoteId",
                table: "ProdutoSerie",
                column: "ProdutoLoteLoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutoSerie_ProdutoLote_ProdutoLoteLoteId",
                table: "ProdutoSerie",
                column: "ProdutoLoteLoteId",
                principalTable: "ProdutoLote",
                principalColumn: "LoteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProdutoFornecedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProdutoFornecedor",
                columns: table => new
                {
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdFornecedor = table.Column<int>(type: "int", nullable: false),
                    PrecoFornecedor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LeadTimeDias = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoFornecedor", x => new { x.IdProduto, x.IdFornecedor });
                    table.ForeignKey(
                        name: "FK_ProdutoFornecedor_Fornecedores_IdFornecedor",
                        column: x => x.IdFornecedor,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoFornecedor_Produto_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produto",
                        principalColumn: "idProduto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_IdFornecedor",
                table: "ProdutoFornecedor",
                column: "IdFornecedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutoFornecedor");
        }
    }
}

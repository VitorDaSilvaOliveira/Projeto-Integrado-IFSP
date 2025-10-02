using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProdutoLoteSerie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadeEstoque",
                table: "Produto");

            migrationBuilder.AddColumn<bool>(
                name: "Rastreio",
                table: "Produto",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Devolucao",
                columns: table => new
                {
                    idDevolucao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDevolucao = table.Column<byte>(type: "tinyint", nullable: false),
                    IdPedido = table.Column<int>(type: "int", nullable: true),
                    IdFornecedor = table.Column<int>(type: "int", nullable: true),
                    DataDevolucao = table.Column<DateTime>(type: "date", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Id_User = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Devolvido = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devolucao", x => x.idDevolucao);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoLote",
                columns: table => new
                {
                    LoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    FornecedorId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoLote", x => x.LoteId);
                    table.ForeignKey(
                        name: "FK_ProdutoLote_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoLote_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "idProduto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DevolucaoItem",
                columns: table => new
                {
                    idDevolucaoItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDevolucao = table.Column<int>(type: "int", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdPedidoItem = table.Column<int>(type: "int", nullable: true),
                    QuantidadeDevolvida = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Devolvido = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevolucaoItem", x => x.idDevolucaoItem);
                    table.ForeignKey(
                        name: "FK_DevolucaoItem_Devolucao_IdDevolucao",
                        column: x => x.IdDevolucao,
                        principalTable: "Devolucao",
                        principalColumn: "idDevolucao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoSerie",
                columns: table => new
                {
                    SerieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    NumeroSerie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProdutoLoteLoteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoSerie", x => x.SerieId);
                    table.ForeignKey(
                        name: "FK_ProdutoSerie_ProdutoLote_ProdutoLoteLoteId",
                        column: x => x.ProdutoLoteLoteId,
                        principalTable: "ProdutoLote",
                        principalColumn: "LoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevolucaoItem_IdDevolucao",
                table: "DevolucaoItem",
                column: "IdDevolucao");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoLote_FornecedorId",
                table: "ProdutoLote",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoLote_ProdutoId",
                table: "ProdutoLote",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoSerie_ProdutoLoteLoteId",
                table: "ProdutoSerie",
                column: "ProdutoLoteLoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevolucaoItem");

            migrationBuilder.DropTable(
                name: "ProdutoSerie");

            migrationBuilder.DropTable(
                name: "Devolucao");

            migrationBuilder.DropTable(
                name: "ProdutoLote");

            migrationBuilder.DropColumn(
                name: "Rastreio",
                table: "Produto");

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeEstoque",
                table: "Produto",
                type: "int",
                nullable: true);
        }
    }
}

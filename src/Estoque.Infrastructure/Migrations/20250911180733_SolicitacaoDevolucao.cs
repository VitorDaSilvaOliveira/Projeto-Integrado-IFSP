using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SolicitacaoDevolucao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitacaoDevolucao",
                columns: table => new
                {
                    IdSolicitacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Devolvido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacaoDevolucao", x => x.IdSolicitacao);
                    table.ForeignKey(
                        name: "FK_SolicitacaoDevolucao_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitacaoDevolucao_Produto_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produto",
                        principalColumn: "idProduto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacaoDevolucao_IdCliente",
                table: "SolicitacaoDevolucao",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacaoDevolucao_IdProduto",
                table: "SolicitacaoDevolucao",
                column: "IdProduto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacaoDevolucao");
        }
    }
}

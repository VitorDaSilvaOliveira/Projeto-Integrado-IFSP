using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrecoesPosMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "PedidoItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Processado = table.Column<bool>(type: "bit", nullable: false),
                    IdRegistro = table.Column<int>(type: "int", nullable: true),
                    Campo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorAntigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorNovo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Id_Categoria",
                table: "Produto",
                column: "Id_Categoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Categoria_Id_Categoria",
                table: "Produto",
                column: "Id_Categoria",
                principalTable: "Categoria",
                principalColumn: "idCategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Categoria_Id_Categoria",
                table: "Produto");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Produto_Id_Categoria",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "PedidoItem");
        }
    }
}

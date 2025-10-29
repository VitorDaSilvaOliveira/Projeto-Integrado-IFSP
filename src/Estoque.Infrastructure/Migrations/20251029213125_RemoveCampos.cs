using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFornecedor",
                table: "Devolucao");

            migrationBuilder.DropColumn(
                name: "IdPedido",
                table: "Devolucao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFornecedor",
                table: "Devolucao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPedido",
                table: "Devolucao",
                type: "int",
                nullable: true);
        }
    }
}

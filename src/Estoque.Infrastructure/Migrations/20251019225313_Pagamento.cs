using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Pagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormaPagamento",
                table: "Pedido",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Parcelas",
                table: "Pedido",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Parcelas",
                table: "Pedido");
        }
    }
}

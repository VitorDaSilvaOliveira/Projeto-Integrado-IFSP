using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "MasterData",
                schema: "dbo",
                columns: table => new
                {
                    name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    type = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    tablename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    info = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    owner = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterData", x => x.name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasterData",
                schema: "dbo");
        }
    }
}

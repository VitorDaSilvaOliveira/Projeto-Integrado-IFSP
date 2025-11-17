using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estoque.Infrastructure.Migrations
{
    public partial class UserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnableLanguageSwitch = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EnableDarkModeSwitch = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", nullable: true, defaultValue: "pt-BR"),
                    DarkMode = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
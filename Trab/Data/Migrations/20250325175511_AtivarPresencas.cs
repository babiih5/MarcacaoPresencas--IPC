using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtivarPresencas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PresencasAtivas",
                table: "Turmas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PresencasAtivas",
                table: "Turmas");
        }
    }
}

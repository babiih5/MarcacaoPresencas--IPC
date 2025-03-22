using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trab.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Sala_DiaSemana : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaSemana",
                table: "Turmas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sala",
                table: "Turmas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaSemana",
                table: "Turmas");

            migrationBuilder.DropColumn(
                name: "Sala",
                table: "Turmas");
        }
    }
}

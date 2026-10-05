using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab06fEFCoreConcurrencia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVersionConcurrencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Turnos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Turnos");
        }
    }
}

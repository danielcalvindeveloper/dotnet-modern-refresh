using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab06cEFCoreRelationsMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTelefonoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Clientes");
        }
    }
}

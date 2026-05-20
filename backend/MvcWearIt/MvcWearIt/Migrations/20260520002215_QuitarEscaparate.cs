using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWearIt.Migrations
{
    /// <inheritdoc />
    public partial class QuitarEscaparate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Escaparate",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Escaparate",
                table: "Productos",
                type: "boolean",
                nullable: true);
        }
    }
}

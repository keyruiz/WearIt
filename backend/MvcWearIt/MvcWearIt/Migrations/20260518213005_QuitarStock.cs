using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWearIt.Migrations
{
    /// <inheritdoc />
    public partial class QuitarStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Productos",
                type: "integer",
                nullable: true);
        }
    }
}

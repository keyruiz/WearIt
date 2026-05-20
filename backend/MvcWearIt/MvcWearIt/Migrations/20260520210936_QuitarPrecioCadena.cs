using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWearIt.Migrations
{
    /// <inheritdoc />
    public partial class QuitarPrecioCadena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioCadena",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrecioCadena",
                table: "Productos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

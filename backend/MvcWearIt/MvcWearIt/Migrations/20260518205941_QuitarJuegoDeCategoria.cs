using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcWearIt.Migrations
{
    /// <inheritdoc />
    public partial class QuitarJuegoDeCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Juegos_JuegoId",
                table: "Categorias");

            migrationBuilder.AlterColumn<int>(
                name: "JuegoId",
                table: "Categorias",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Juegos_JuegoId",
                table: "Categorias",
                column: "JuegoId",
                principalTable: "Juegos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

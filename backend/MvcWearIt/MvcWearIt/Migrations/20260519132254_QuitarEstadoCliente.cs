using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MvcWearIt.Migrations
{
    /// <inheritdoc />
    public partial class QuitarEstadoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"Clientes\" CASCADE");

            migrationBuilder.Sql("DROP TABLE IF EXISTS \"Estados\" CASCADE");

            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Pedidos_ClienteId\"");

            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Pedidos_EstadoId\"");

            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Anulado\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"ClienteId\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Cobrado\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Confirmado\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Devuelto\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Enviado\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"EstadoId\"");
            migrationBuilder.Sql("ALTER TABLE \"Pedidos\" DROP COLUMN IF EXISTS \"Preparado\"");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Pedidos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pedidos");

            migrationBuilder.AddColumn<DateTime>(
                name: "Anulado",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Pedidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Cobrado",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Confirmado",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Devuelto",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Enviado",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Pedidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Preparado",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Nif = table.Column<string>(type: "text", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Poblacion = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteId",
                table: "Pedidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EstadoId",
                table: "Pedidos",
                column: "EstadoId");


        }
    }
}

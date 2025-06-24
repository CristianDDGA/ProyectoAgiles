using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateDiticTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DITIC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NombreCapacitacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TipoCapacitacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Presencial"),
                    HorasAcademicas = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Completada"),
                    Calificacion = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CalificacionMinima = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 70m),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NumeroCertificado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Instructor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ArchivoCertificado = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NombreArchivoCertificado = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExencionPorAutoridad = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CargoAutoridad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaInicioAutoridad = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinAutoridad = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DITIC", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DITIC_Anio",
                table: "DITIC",
                column: "Anio");

            migrationBuilder.CreateIndex(
                name: "IX_DITIC_Cedula",
                table: "DITIC",
                column: "Cedula");

            migrationBuilder.CreateIndex(
                name: "IX_DITIC_Cedula_NombreCapacitacion_Institucion_FechaInicio",
                table: "DITIC",
                columns: new[] { "Cedula", "NombreCapacitacion", "Institucion", "FechaInicio" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DITIC");
        }
    }
}

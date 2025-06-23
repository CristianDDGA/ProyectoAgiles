using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaEvaluacionesDesempeno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluacionesDesempeno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PeriodoAcademico = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Semestre = table.Column<int>(type: "int", nullable: false),
                    PuntajeObtenido = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PuntajeMaximo = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 100m),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEvaluacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Integral"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Completada"),
                    Evaluador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchivoRespaldo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NombreArchivoRespaldo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesDesempeno", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesDesempeno_Cedula_PeriodoAcademico",
                table: "EvaluacionesDesempeno",
                columns: new[] { "Cedula", "PeriodoAcademico" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionesDesempeno");
        }
    }
}

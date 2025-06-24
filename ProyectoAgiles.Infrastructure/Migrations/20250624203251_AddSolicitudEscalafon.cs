using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudEscalafon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesEscalafon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DocenteNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocenteEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocenteTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Facultad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Carrera = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NivelActual = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NivelSolicitado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnosExperiencia = table.Column<int>(type: "int", nullable: false),
                    Titulos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Publicaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProyectosInvestigacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Capacitaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRechazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProcesadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesEscalafon", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEscalafon_DocenteCedula",
                table: "SolicitudesEscalafon",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEscalafon_Status",
                table: "SolicitudesEscalafon",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesEscalafon");
        }
    }
}

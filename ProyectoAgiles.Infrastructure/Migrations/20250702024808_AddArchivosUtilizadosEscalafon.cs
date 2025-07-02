using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivosUtilizadosEscalafon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivosUtilizadosEscalafon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudEscalafonId = table.Column<int>(type: "int", nullable: false),
                    TipoRecurso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecursoId = table.Column<int>(type: "int", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NivelOrigen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NivelDestino = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaUtilizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstadoAscenso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivosUtilizadosEscalafon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivosUtilizadosEscalafon_SolicitudesEscalafon_SolicitudEscalafonId",
                        column: x => x.SolicitudEscalafonId,
                        principalTable: "SolicitudesEscalafon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosUtilizadosEscalafon_DocenteCedula",
                table: "ArchivosUtilizadosEscalafon",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosUtilizadosEscalafon_DocenteCedula_TipoRecurso",
                table: "ArchivosUtilizadosEscalafon",
                columns: new[] { "DocenteCedula", "TipoRecurso" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosUtilizadosEscalafon_SolicitudEscalafonId",
                table: "ArchivosUtilizadosEscalafon",
                column: "SolicitudEscalafonId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosUtilizadosEscalafon_TipoRecurso_RecursoId",
                table: "ArchivosUtilizadosEscalafon",
                columns: new[] { "TipoRecurso", "RecursoId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivosUtilizadosEscalafon");
        }
    }
}

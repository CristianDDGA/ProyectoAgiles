using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodoPostulacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodoPostulacionId",
                table: "SolicitudesEscalafon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PeriodosPostulacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosPostulacion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEscalafon_PeriodoPostulacionId",
                table: "SolicitudesEscalafon",
                column: "PeriodoPostulacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosPostulacion_Activo",
                table: "PeriodosPostulacion",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosPostulacion_FechaInicio_FechaFin",
                table: "PeriodosPostulacion",
                columns: new[] { "FechaInicio", "FechaFin" });

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesEscalafon_PeriodosPostulacion_PeriodoPostulacionId",
                table: "SolicitudesEscalafon",
                column: "PeriodoPostulacionId",
                principalTable: "PeriodosPostulacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesEscalafon_PeriodosPostulacion_PeriodoPostulacionId",
                table: "SolicitudesEscalafon");

            migrationBuilder.DropTable(
                name: "PeriodosPostulacion");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesEscalafon_PeriodoPostulacionId",
                table: "SolicitudesEscalafon");

            migrationBuilder.DropColumn(
                name: "PeriodoPostulacionId",
                table: "SolicitudesEscalafon");
        }
    }
}

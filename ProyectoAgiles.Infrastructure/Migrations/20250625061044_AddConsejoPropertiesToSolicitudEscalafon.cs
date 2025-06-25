using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsejoPropertiesToSolicitudEscalafon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEnvioConsejo",
                table: "SolicitudesEscalafon",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazoConsejo",
                table: "SolicitudesEscalafon",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesConsejo",
                table: "SolicitudesEscalafon",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaEnvioConsejo",
                table: "SolicitudesEscalafon");

            migrationBuilder.DropColumn(
                name: "MotivoRechazoConsejo",
                table: "SolicitudesEscalafon");

            migrationBuilder.DropColumn(
                name: "ObservacionesConsejo",
                table: "SolicitudesEscalafon");
        }
    }
}

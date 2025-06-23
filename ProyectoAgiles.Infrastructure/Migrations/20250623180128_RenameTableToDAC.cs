using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableToDAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluacionesDesempeno",
                table: "EvaluacionesDesempeno");

            migrationBuilder.RenameTable(
                name: "EvaluacionesDesempeno",
                newName: "DAC");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluacionesDesempeno_Cedula_PeriodoAcademico",
                table: "DAC",
                newName: "IX_DAC_Cedula_PeriodoAcademico");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DAC",
                table: "DAC",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DAC",
                table: "DAC");

            migrationBuilder.RenameTable(
                name: "DAC",
                newName: "EvaluacionesDesempeno");

            migrationBuilder.RenameIndex(
                name: "IX_DAC_Cedula_PeriodoAcademico",
                table: "EvaluacionesDesempeno",
                newName: "IX_EvaluacionesDesempeno_Cedula_PeriodoAcademico");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluacionesDesempeno",
                table: "EvaluacionesDesempeno",
                column: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTTHH_CedulaFechaInicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TTHH_Users_UserId",
                table: "TTHH");

            migrationBuilder.DropIndex(
                name: "IX_TTHH_UserId",
                table: "TTHH");

            migrationBuilder.DropColumn(
                name: "Anio",
                table: "TTHH");

            migrationBuilder.DropColumn(
                name: "TiempoCumplido",
                table: "TTHH");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TTHH");

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "TTHH",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "TTHH",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "TTHH");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "TTHH");

            migrationBuilder.AddColumn<int>(
                name: "Anio",
                table: "TTHH",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TiempoCumplido",
                table: "TTHH",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TTHH",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TTHH_UserId",
                table: "TTHH",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TTHH_Users_UserId",
                table: "TTHH",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

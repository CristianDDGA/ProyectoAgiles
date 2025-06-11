using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalTeacherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalTeachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Universidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombresCompletos = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalTeachers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ExternalTeachers",
                columns: new[] { "Id", "Cedula", "CreatedAt", "NombresCompletos", "Universidad", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "1750000001", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(6911), "María Elena García Pérez", "Universidad Técnica de Ambato", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7077) },
                    { 2, "1750000002", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7214), "Carlos Alberto Mendoza Silva", "Universidad Técnica de Ambato", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7214) },
                    { 3, "1750000003", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7215), "Ana Cristina López Vargas", "Universidad Técnica de Ambato", new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7216) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalTeachers_Cedula",
                table: "ExternalTeachers",
                column: "Cedula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalTeachers");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixExternalTeacherSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(6911), new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7077) });

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7214), new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7214) });

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7215), new DateTime(2025, 6, 11, 16, 5, 1, 997, DateTimeKind.Utc).AddTicks(7216) });
        }
    }
}

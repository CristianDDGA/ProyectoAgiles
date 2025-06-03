using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$xQVm8QpRGyV.rqm/JJt7p.3J7N6pC0qFb0q5RHbj6h8D2Cz0C.L9i" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 3, 14, 51, 16, 503, DateTimeKind.Utc).AddTicks(7040), "$2a$11$FxRNwoMAsGTBFnaVVqtpnOHPhcMwRBDQp2t6nCY0k.77Fndo22cqS" });
        }
    }
}

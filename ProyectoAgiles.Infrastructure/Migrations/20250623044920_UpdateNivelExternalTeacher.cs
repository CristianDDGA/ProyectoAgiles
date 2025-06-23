using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNivelExternalTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "TTHH",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TTHH",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nivel",
                table: "ExternalTeachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nivel",
                value: "no autoridad");

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nivel",
                value: "no autoridad");

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nivel",
                value: "no autoridad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TTHH");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "ExternalTeachers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "TTHH",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

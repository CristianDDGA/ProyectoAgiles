using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoAgiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNivelFromExternalTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "ExternalTeachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                value: "");

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nivel",
                value: "");

            migrationBuilder.UpdateData(
                table: "ExternalTeachers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nivel",
                value: "");
        }
    }
}

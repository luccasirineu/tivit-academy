using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusMatricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusPagamento",
                table: "Matriculas",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Matriculas",
                newName: "StatusPagamento");
        }
    }
}

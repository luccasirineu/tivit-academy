using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteChamadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamadas_Matriculas_MatriculaId",
                table: "Chamadas");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamadas_Matriculas_MatriculaId",
                table: "Chamadas",
                column: "MatriculaId",
                principalTable: "Matriculas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamadas_Matriculas_MatriculaId",
                table: "Chamadas");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamadas_Matriculas_MatriculaId",
                table: "Chamadas",
                column: "MatriculaId",
                principalTable: "Matriculas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

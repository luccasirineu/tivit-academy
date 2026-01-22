using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class atualizandoConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurmaId",
                table: "Chamadas",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chamadas_TurmaId",
                table: "Chamadas",
                column: "TurmaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamadas_Turmas_TurmaId",
                table: "Chamadas",
                column: "TurmaId",
                principalTable: "Turmas",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove FK
            migrationBuilder.DropForeignKey(
                name: "FK_Chamadas_Turmas_TurmaId",
                table: "Chamadas");

            // Remove índice
            migrationBuilder.DropIndex(
                name: "IX_Chamadas_TurmaId",
                table: "Chamadas");

            // Remove coluna
            migrationBuilder.DropColumn(
                name: "TurmaId",
                table: "Chamadas");
        }
    }
}

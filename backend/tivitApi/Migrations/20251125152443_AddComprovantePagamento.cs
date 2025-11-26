using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class AddComprovantePagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComprovantesPagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatriculaId = table.Column<int>(type: "int", nullable: false),
                    Arquivo = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    HoraEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprovantesPagamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprovantesPagamento_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprovantesPagamento_MatriculaId",
                table: "ComprovantesPagamento",
                column: "MatriculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComprovantesPagamento");
        }
    }
}

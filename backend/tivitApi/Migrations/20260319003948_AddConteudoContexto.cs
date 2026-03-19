using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConteudoContexto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConteudosContexto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConteudoId = table.Column<int>(type: "int", nullable: false),
                    ContextoTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContextoEmbedding = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataArmazenamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TamanhoTokens = table.Column<int>(type: "int", nullable: false),
                    StatusExtracao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MensagemErro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TurmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConteudosContexto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConteudosContexto_Conteudos_ConteudoId",
                        column: x => x.ConteudoId,
                        principalTable: "Conteudos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConteudosContexto_ConteudoId",
                table: "ConteudosContexto",
                column: "ConteudoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConteudosContexto");
        }
    }
}

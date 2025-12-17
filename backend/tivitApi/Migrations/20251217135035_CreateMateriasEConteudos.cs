using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    public partial class CreateMateriasEConteudos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // TABELA: Materias
            // =========================
            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Nome = table.Column<string>(nullable: false),
                    Descricao = table.Column<string>(nullable: false),

                    CursoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Materias_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materias_CursoId",
                table: "Materias",
                column: "CursoId"
            );

            // =========================
            // TABELA: Conteudos
            // =========================
            migrationBuilder.CreateTable(
                name: "Conteudos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Titulo = table.Column<string>(nullable: false),
                    Tipo = table.Column<string>(nullable: false),
                    UrlArquivo = table.Column<string>(nullable: false),

                    DataPublicacao = table.Column<DateTime>(nullable: false),

                    MateriaId = table.Column<int>(nullable: false),
                    ProfessorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conteudos", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Conteudos_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );

                    table.ForeignKey(
                        name: "FK_Conteudos_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction
                    );
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conteudos_MateriaId",
                table: "Conteudos",
                column: "MateriaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Conteudos_ProfessorId",
                table: "Conteudos",
                column: "ProfessorId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conteudos"
            );

            migrationBuilder.DropTable(
                name: "Materias"
            );
        }
    }
}


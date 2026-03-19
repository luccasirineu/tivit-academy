using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class EdidantoConteudoContextos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextoEmbedding",
                table: "ConteudosContexto");

            migrationBuilder.DropColumn(
                name: "TamanhoTokens",
                table: "ConteudosContexto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContextoEmbedding",
                table: "ConteudosContexto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TamanhoTokens",
                table: "ConteudosContexto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

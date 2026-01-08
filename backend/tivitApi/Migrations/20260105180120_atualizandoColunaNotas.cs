using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class atualizandoColunaNotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Media",
                table: "Notas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Media",
                table: "Notas");
        }
    }
}

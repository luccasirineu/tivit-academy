using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tivitApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusPagamento",
                table: "Matriculas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusPagamento",
                table: "Matriculas");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base_de_dados_Fonoaudiologia.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarParecerIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParecerIA",
                table: "DocumentosGerados",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParecerIA",
                table: "DocumentosGerados");
        }
    }
}

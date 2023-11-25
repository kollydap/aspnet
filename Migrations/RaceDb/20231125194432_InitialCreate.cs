using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webutvikling.Migrations.RaceDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WinnerName = table.Column<string>(type: "TEXT", nullable: false),
                    WinnerTime = table.Column<string>(type: "TEXT", nullable: false),
                    GrandPrix = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfLaps = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Races");
        }
    }
}

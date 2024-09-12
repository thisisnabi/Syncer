using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syncer.APIs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinerToPresentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Joiners",
                table: "Presentations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Joiners",
                table: "Presentations");
        }
    }
}

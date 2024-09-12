using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syncer.APIs.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShortnameFromMilstoneEmoji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "MilestoneEmojis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "MilestoneEmojis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

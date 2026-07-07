using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummerBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildIdColumnToBucketListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "BucketListItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "BucketListItems");
        }
    }
}

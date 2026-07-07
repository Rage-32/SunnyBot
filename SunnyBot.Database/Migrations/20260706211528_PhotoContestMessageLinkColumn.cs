using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummerBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class PhotoContestMessageLinkColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageLink",
                table: "PhotoSubmissions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageLink",
                table: "PhotoSubmissions");
        }
    }
}

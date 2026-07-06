using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummerBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoVoteUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PhotoVotes_PhotoSubmissionId_UserId",
                table: "PhotoVotes",
                columns: new[] { "PhotoSubmissionId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhotoVotes_PhotoSubmissionId_UserId",
                table: "PhotoVotes");
        }
    }
}

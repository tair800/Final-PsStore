using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class commentCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReaction_Comments_CommentId",
                table: "CommentReaction");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReaction_Comments_CommentId",
                table: "CommentReaction",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReaction_Comments_CommentId",
                table: "CommentReaction");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReaction_Comments_CommentId",
                table: "CommentReaction",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class dlcBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DlcId",
                table: "BasketGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BasketGames_DlcId",
                table: "BasketGames",
                column: "DlcId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames",
                column: "DlcId",
                principalTable: "Dlcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames");

            migrationBuilder.DropIndex(
                name: "IX_BasketGames_DlcId",
                table: "BasketGames");

            migrationBuilder.DropColumn(
                name: "DlcId",
                table: "BasketGames");
        }
    }
}

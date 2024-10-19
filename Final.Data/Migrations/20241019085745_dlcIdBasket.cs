using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class dlcIdBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames",
                column: "DlcId",
                principalTable: "Dlcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketGames_Dlcs_DlcId",
                table: "BasketGames",
                column: "DlcId",
                principalTable: "Dlcs",
                principalColumn: "Id");
        }
    }
}

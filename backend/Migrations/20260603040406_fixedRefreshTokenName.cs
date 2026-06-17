using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildSync.Migrations
{
    /// <inheritdoc />
    public partial class fixedRefreshTokenName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokensId",
                table: "RefreshTokens",
                newName: "RefreshTokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenId",
                table: "RefreshTokens",
                newName: "RefreshTokensId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToRefreshTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_user_refresh_token",
                table: "users",
                column: "user_refresh_token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_user_refresh_token",
                table: "users");
        }
    }
}

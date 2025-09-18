using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class UserAndRolesTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_roles_RolesId",
                table: "users_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_users_UsersId",
                table: "users_roles");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "users_roles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "users_roles",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_roles_UsersId",
                table: "users_roles",
                newName: "IX_users_roles_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_roles_role_id",
                table: "users_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_users_user_id",
                table: "users_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_uuid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_roles_role_id",
                table: "users_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_users_user_id",
                table: "users_roles");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "users_roles",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "users_roles",
                newName: "RolesId");

            migrationBuilder.RenameIndex(
                name: "IX_users_roles_user_id",
                table: "users_roles",
                newName: "IX_users_roles_UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_roles_RolesId",
                table: "users_roles",
                column: "RolesId",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_users_UsersId",
                table: "users_roles",
                column: "UsersId",
                principalTable: "users",
                principalColumn: "user_uuid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

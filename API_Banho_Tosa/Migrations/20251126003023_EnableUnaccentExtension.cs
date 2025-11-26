using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class EnableUnaccentExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_service_status_service_status_id",
                table: "services");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "services",
                newName: "service_uuid");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddForeignKey(
                name: "FK_services_service_status_service_status_id",
                table: "services",
                column: "service_status_id",
                principalTable: "service_status",
                principalColumn: "service_status_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_service_status_service_status_id",
                table: "services");

            migrationBuilder.RenameColumn(
                name: "service_uuid",
                table: "services",
                newName: "Uuid");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddForeignKey(
                name: "FK_services_service_status_service_status_id",
                table: "services",
                column: "service_status_id",
                principalTable: "service_status",
                principalColumn: "service_status_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_payment_status_payment_status_id",
                table: "services");

            migrationBuilder.AddForeignKey(
                name: "FK_services_payment_status_payment_status_id",
                table: "services",
                column: "payment_status_id",
                principalTable: "payment_status",
                principalColumn: "payment_status_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_services_payment_status_payment_status_id",
                table: "services");

            migrationBuilder.AddForeignKey(
                name: "FK_services_payment_status_payment_status_id",
                table: "services",
                column: "payment_status_id",
                principalTable: "payment_status",
                principalColumn: "payment_status_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

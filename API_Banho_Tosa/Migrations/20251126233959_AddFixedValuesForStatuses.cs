using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddFixedValuesForStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "payment_status",
                columns: new[] { "payment_status_id", "payment_status_description" },
                values: new object[,]
                {
                    { 1, "AGUARDANDO PAGAMENTO" },
                    { 2, "PAGAMENTO CONFIRMADO" },
                    { 3, "CANCELADO PELO USUÁRIO" }
                });

            migrationBuilder.InsertData(
                table: "service_status",
                columns: new[] { "service_status_id", "service_status_description" },
                values: new object[,]
                {
                    { 1, "SERVIÇO AGENDADO" },
                    { 2, "SERVIÇO CONCLUIDO" },
                    { 3, "SERVIÇO CANCELADO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "payment_status",
                keyColumn: "payment_status_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "payment_status",
                keyColumn: "payment_status_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "payment_status",
                keyColumn: "payment_status_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "service_status",
                keyColumn: "service_status_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "service_status",
                keyColumn: "service_status_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "service_status",
                keyColumn: "service_status_id",
                keyValue: 3);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "available_services",
                columns: table => new
                {
                    available_service_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    available_service_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    available_service_duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_available_services", x => x.available_service_id);
                });

            migrationBuilder.CreateTable(
                name: "payment_status",
                columns: table => new
                {
                    payment_status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    payment_status_description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_status", x => x.payment_status_id);
                });

            migrationBuilder.CreateTable(
                name: "service_status",
                columns: table => new
                {
                    service_status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_status_description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_status", x => x.service_status_id);
                });

            migrationBuilder.CreateTable(
                name: "service_prices",
                columns: table => new
                {
                    available_service_id = table.Column<int>(type: "integer", nullable: false),
                    pet_size_id = table.Column<int>(type: "integer", nullable: false),
                    service_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_prices", x => new { x.available_service_id, x.pet_size_id });
                    table.ForeignKey(
                        name: "FK_service_prices_available_services_available_service_id",
                        column: x => x.available_service_id,
                        principalTable: "available_services",
                        principalColumn: "available_service_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_prices_pet_sizes_pet_size_id",
                        column: x => x.pet_size_id,
                        principalTable: "pet_sizes",
                        principalColumn: "pet_size_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    service_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_status_id = table.Column<int>(type: "integer", nullable: false),
                    payment_status_id = table.Column<int>(type: "integer", nullable: false),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payment_due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    additional_charges = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    final_total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.service_id);
                    table.ForeignKey(
                        name: "FK_services_payment_status_payment_status_id",
                        column: x => x.payment_status_id,
                        principalTable: "payment_status",
                        principalColumn: "payment_status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_services_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "pet_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_services_service_status_service_status_id",
                        column: x => x.service_status_id,
                        principalTable: "service_status",
                        principalColumn: "service_status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "service_items",
                columns: table => new
                {
                    service_item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    available_service = table.Column<int>(type: "integer", nullable: false),
                    price_at_the_time = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_items", x => x.service_item_id);
                    table.ForeignKey(
                        name: "FK_service_items_available_services_available_service",
                        column: x => x.available_service,
                        principalTable: "available_services",
                        principalColumn: "available_service_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_items_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment_status_payment_status_description",
                table: "payment_status",
                column: "payment_status_description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_items_available_service",
                table: "service_items",
                column: "available_service");

            migrationBuilder.CreateIndex(
                name: "IX_service_items_service_id",
                table: "service_items",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_prices_pet_size_id",
                table: "service_prices",
                column: "pet_size_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_status_service_status_description",
                table: "service_status",
                column: "service_status_description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_services_payment_status_id",
                table: "services",
                column: "payment_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_pet_id",
                table: "services",
                column: "pet_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_service_status_id",
                table: "services",
                column: "service_status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_items");

            migrationBuilder.DropTable(
                name: "service_prices");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "available_services");

            migrationBuilder.DropTable(
                name: "payment_status");

            migrationBuilder.DropTable(
                name: "service_status");
        }
    }
}

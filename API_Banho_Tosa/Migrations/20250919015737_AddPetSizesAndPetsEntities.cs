using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddPetSizesAndPetsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pet_sizes",
                columns: table => new
                {
                    pet_size_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pet_size_description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_sizes", x => x.pet_size_id);
                });

            migrationBuilder.CreateTable(
                name: "pets",
                columns: table => new
                {
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pet_name = table.Column<string>(type: "text", nullable: false),
                    breed_id = table.Column<int>(type: "integer", nullable: false),
                    pet_size_id = table.Column<int>(type: "integer", nullable: false),
                    pet_birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pet_latest_visit = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets", x => x.pet_id);
                    table.ForeignKey(
                        name: "FK_pets_breeds_breed_id",
                        column: x => x.breed_id,
                        principalTable: "breeds",
                        principalColumn: "breed_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pets_pet_sizes_pet_size_id",
                        column: x => x.pet_size_id,
                        principalTable: "pet_sizes",
                        principalColumn: "pet_size_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pets_owners",
                columns: table => new
                {
                    owner_id = table.Column<int>(type: "integer", nullable: false),
                    pet_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pets_owners", x => new { x.owner_id, x.pet_id });
                    table.ForeignKey(
                        name: "FK_pets_owners_owners_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owners",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pets_owners_pets_pet_id",
                        column: x => x.pet_id,
                        principalTable: "pets",
                        principalColumn: "pet_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pets_breed_id",
                table: "pets",
                column: "breed_id");

            migrationBuilder.CreateIndex(
                name: "IX_pets_pet_size_id",
                table: "pets",
                column: "pet_size_id");

            migrationBuilder.CreateIndex(
                name: "IX_pets_owners_pet_id",
                table: "pets_owners",
                column: "pet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pets_owners");

            migrationBuilder.DropTable(
                name: "pets");

            migrationBuilder.DropTable(
                name: "pet_sizes");
        }
    }
}

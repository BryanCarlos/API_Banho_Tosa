using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_Banho_Tosa.Migrations
{
    /// <inheritdoc />
    public partial class AddBreedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "breeds",
                columns: table => new
                {
                    breed_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    breed_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    animal_type_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_breeds", x => x.breed_id);
                    table.ForeignKey(
                        name: "FK_breeds_animal_types_animal_type_id",
                        column: x => x.animal_type_id,
                        principalTable: "animal_types",
                        principalColumn: "animal_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_breeds_animal_type_id",
                table: "breeds",
                column: "animal_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "breeds");
        }
    }
}

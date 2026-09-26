using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCompetencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_competencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_competencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_competencies_competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "competencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_competencies_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_competencies_CompetencyId",
                table: "user_competencies",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "IX_user_competencies_UserId_CompetencyId",
                table: "user_competencies",
                columns: new[] { "UserId", "CompetencyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_competencies");
        }
    }
}

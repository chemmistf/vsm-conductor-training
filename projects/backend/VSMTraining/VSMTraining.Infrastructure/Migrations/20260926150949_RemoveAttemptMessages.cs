using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttemptMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attempt_messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attempt_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EvaluationJson = table.Column<string>(type: "jsonb", nullable: true),
                    Model = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NodeId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempt_messages_attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attempt_messages_AttemptId",
                table: "attempt_messages",
                column: "AttemptId");
        }
    }
}

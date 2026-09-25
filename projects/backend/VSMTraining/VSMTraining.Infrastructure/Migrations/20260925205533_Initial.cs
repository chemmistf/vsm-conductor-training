using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "competencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Depot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Brigade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Xp = table.Column<long>(type: "bigint", nullable: false),
                    CurrentServiceClass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CertificationStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scenario_versions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScenarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ContentJson = table.Column<string>(type: "jsonb", nullable: false),
                    SourceDescription = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scenario_versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scenario_versions_scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScenarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScenarioVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetServiceClass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CurrentNodeId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InitialSafety = table.Column<int>(type: "integer", nullable: false),
                    CurrentSafety = table.Column<int>(type: "integer", nullable: false),
                    FinalSafety = table.Column<int>(type: "integer", nullable: true),
                    InitialLoyalty = table.Column<int>(type: "integer", nullable: false),
                    CurrentLoyalty = table.Column<int>(type: "integer", nullable: false),
                    FinalLoyalty = table.Column<int>(type: "integer", nullable: true),
                    ResultStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CriticalErrorsCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ActiveModifiersJson = table.Column<string>(type: "jsonb", nullable: true),
                    SelectedVariantsJson = table.Column<string>(type: "jsonb", nullable: true),
                    ContextJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempts_scenario_versions_ScenarioVersionId",
                        column: x => x.ScenarioVersionId,
                        principalTable: "scenario_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attempts_scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attempts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attempt_competencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompetencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositiveSignals = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    NegativeSignals = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_competencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempt_competencies_attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attempt_competencies_competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "competencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attempt_events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NodeVariantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChoiceId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResponseTimeMs = table.Column<int>(type: "integer", nullable: true),
                    SafetyBefore = table.Column<int>(type: "integer", nullable: true),
                    SafetyDelta = table.Column<int>(type: "integer", nullable: true),
                    SafetyAfter = table.Column<int>(type: "integer", nullable: true),
                    LoyaltyBefore = table.Column<int>(type: "integer", nullable: true),
                    LoyaltyDelta = table.Column<int>(type: "integer", nullable: true),
                    LoyaltyAfter = table.Column<int>(type: "integer", nullable: true),
                    CriticalError = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CriticalErrorCode = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EventDataJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attempt_events_attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attempt_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    NodeId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EvaluationJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "certifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceClass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certifications_attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_certifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attempt_competencies_AttemptId_CompetencyId",
                table: "attempt_competencies",
                columns: new[] { "AttemptId", "CompetencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_attempt_competencies_CompetencyId",
                table: "attempt_competencies",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_events_AttemptId",
                table: "attempt_events",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_events_AttemptId_OccurredAt",
                table: "attempt_events",
                columns: new[] { "AttemptId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_attempt_messages_AttemptId",
                table: "attempt_messages",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_attempts_ResultStatus",
                table: "attempts",
                column: "ResultStatus");

            migrationBuilder.CreateIndex(
                name: "IX_attempts_ScenarioId",
                table: "attempts",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_attempts_ScenarioVersionId",
                table: "attempts",
                column: "ScenarioVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_attempts_UserId",
                table: "attempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_certifications_AttemptId",
                table: "certifications",
                column: "AttemptId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_certifications_UserId",
                table: "certifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_competencies_Code",
                table: "competencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scenario_versions_ScenarioId_Version",
                table: "scenario_versions",
                columns: new[] { "ScenarioId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scenarios_Code",
                table: "scenarios",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_ExternalId",
                table: "users",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attempt_competencies");

            migrationBuilder.DropTable(
                name: "attempt_events");

            migrationBuilder.DropTable(
                name: "attempt_messages");

            migrationBuilder.DropTable(
                name: "certifications");

            migrationBuilder.DropTable(
                name: "competencies");

            migrationBuilder.DropTable(
                name: "attempts");

            migrationBuilder.DropTable(
                name: "scenario_versions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "scenarios");
        }
    }
}

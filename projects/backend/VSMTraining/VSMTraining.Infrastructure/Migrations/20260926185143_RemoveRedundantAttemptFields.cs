using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantAttemptFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempts_scenarios_ScenarioId",
                table: "attempts");

            migrationBuilder.DropIndex(
                name: "IX_attempts_ScenarioId",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "CriticalErrorsCount",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "ScenarioId",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "NodeVariantId",
                table: "attempt_events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CriticalErrorsCount",
                table: "attempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ScenarioId",
                table: "attempts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "NodeVariantId",
                table: "attempt_events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_attempts_ScenarioId",
                table: "attempts",
                column: "ScenarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_attempts_scenarios_ScenarioId",
                table: "attempts",
                column: "ScenarioId",
                principalTable: "scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

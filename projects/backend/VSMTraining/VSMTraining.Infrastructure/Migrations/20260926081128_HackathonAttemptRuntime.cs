using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HackathonAttemptRuntime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResultStatus",
                table: "attempts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CurrentNodeStartedAt",
                table: "attempts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LifecycleStatus",
                table: "attempts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NodeDeadlineAt",
                table: "attempts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_attempts_LifecycleStatus",
                table: "attempts",
                column: "LifecycleStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_attempts_LifecycleStatus",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "CurrentNodeStartedAt",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "LifecycleStatus",
                table: "attempts");

            migrationBuilder.DropColumn(
                name: "NodeDeadlineAt",
                table: "attempts");

            migrationBuilder.AlterColumn<string>(
                name: "ResultStatus",
                table: "attempts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}

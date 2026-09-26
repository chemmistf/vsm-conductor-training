using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VSMTraining.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompetencySignalCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NegativeSignals",
                table: "attempt_competencies");

            migrationBuilder.DropColumn(
                name: "PositiveSignals",
                table: "attempt_competencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NegativeSignals",
                table: "attempt_competencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositiveSignals",
                table: "attempt_competencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRawEventPatientOccurredAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_raw_events_PatientId_OccurredAtUtc",
                table: "raw_events",
                columns: new[] { "PatientId", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_raw_events_PatientId_OccurredAtUtc",
                table: "raw_events");
        }
    }
}

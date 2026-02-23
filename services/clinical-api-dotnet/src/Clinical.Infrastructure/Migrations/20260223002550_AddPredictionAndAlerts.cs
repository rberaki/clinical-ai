using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPredictionAndAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prediction_runs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelVersion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HorizonHours = table.Column<int>(type: "integer", nullable: false),
                    RiskScore = table.Column<double>(type: "double precision", nullable: false),
                    SurvivalSummaryJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prediction_runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prediction_runs_feature_runs_FeatureRunId",
                        column: x => x.FeatureRunId,
                        principalTable: "feature_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PredictionRunId = table.Column<Guid>(type: "uuid", nullable: false),
                    Threshold = table.Column<double>(type: "double precision", nullable: false),
                    RiskScore = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    AcknowledgedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ClosedAtUtc = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alerts_prediction_runs_PredictionRunId",
                        column: x => x.PredictionRunId,
                        principalTable: "prediction_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_EncounterId_Status_CreatedAtUtc",
                table: "alerts",
                columns: new[] { "EncounterId", "Status", "CreatedAtUtc" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_PredictionRunId",
                table: "alerts",
                column: "PredictionRunId");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_runs_FeatureRunId_CreatedAtUtc",
                table: "prediction_runs",
                columns: new[] { "FeatureRunId", "CreatedAtUtc" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "prediction_runs");
        }
    }
}

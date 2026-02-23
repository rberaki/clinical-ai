using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinical.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceIncrementalIngestionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_raw_events_IdempotencyKey",
                table: "raw_events");

            migrationBuilder.RenameColumn(
                name: "SourceSystem",
                table: "raw_events",
                newName: "Source");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OccurredAtUtc",
                table: "raw_events",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IngestedAtUtc",
                table: "raw_events",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "IdempotencyKey",
                table: "raw_events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "EncounterId",
                table: "raw_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "raw_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "pipeline_watermarks",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastProcessedAtUtc",
                table: "pipeline_watermarks",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "WindowStartUtc",
                table: "feature_runs",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "WindowEndUtc",
                table: "feature_runs",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartedAtUtc",
                table: "feature_runs",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                table: "feature_runs",
                type: "timestamptz",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EncounterId",
                table: "feature_runs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FeatureHash",
                table: "feature_runs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeatureVersion",
                table: "feature_runs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid[]>(
                name: "InputEventIds",
                table: "feature_runs",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.CreateIndex(
                name: "IX_raw_events_EncounterId_OccurredAtUtc",
                table: "raw_events",
                columns: new[] { "EncounterId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_raw_events_IdempotencyKey",
                table: "raw_events",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_feature_runs_PipelineName_EncounterId_WindowEndUtc",
                table: "feature_runs",
                columns: new[] { "PipelineName", "EncounterId", "WindowEndUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_raw_events_EncounterId_OccurredAtUtc",
                table: "raw_events");

            migrationBuilder.DropIndex(
                name: "IX_raw_events_IdempotencyKey",
                table: "raw_events");

            migrationBuilder.DropIndex(
                name: "IX_feature_runs_PipelineName_EncounterId_WindowEndUtc",
                table: "feature_runs");

            migrationBuilder.DropColumn(
                name: "EncounterId",
                table: "raw_events");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "raw_events");

            migrationBuilder.DropColumn(
                name: "EncounterId",
                table: "feature_runs");

            migrationBuilder.DropColumn(
                name: "FeatureHash",
                table: "feature_runs");

            migrationBuilder.DropColumn(
                name: "FeatureVersion",
                table: "feature_runs");

            migrationBuilder.DropColumn(
                name: "InputEventIds",
                table: "feature_runs");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "raw_events",
                newName: "SourceSystem");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "OccurredAtUtc",
                table: "raw_events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IngestedAtUtc",
                table: "raw_events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "IdempotencyKey",
                table: "raw_events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "pipeline_watermarks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastProcessedAtUtc",
                table: "pipeline_watermarks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "WindowStartUtc",
                table: "feature_runs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "WindowEndUtc",
                table: "feature_runs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartedAtUtc",
                table: "feature_runs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                table: "feature_runs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamptz",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_raw_events_IdempotencyKey",
                table: "raw_events",
                column: "IdempotencyKey",
                unique: true);
        }
    }
}

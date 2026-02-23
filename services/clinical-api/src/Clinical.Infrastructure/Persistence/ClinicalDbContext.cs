using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Infrastructure.Persistence;

public sealed class ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) : DbContext(options)
{
    public DbSet<RawEvent> RawEvents => Set<RawEvent>();
    public DbSet<PipelineWatermark> PipelineWatermarks => Set<PipelineWatermark>();
    public DbSet<FeatureRun> FeatureRuns => Set<FeatureRun>();
    public DbSet<PredictionRun> PredictionRuns => Set<PredictionRun>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RawEvent>(entity =>
        {
            entity.ToTable("raw_events");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.IdempotencyKey).HasMaxLength(200);
            entity.Property(x => x.PatientId);
            entity.Property(x => x.EncounterId);
            entity.Property(x => x.SourceSystem).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PayloadJson).HasColumnType("jsonb").IsRequired();

            entity.Property(x => x.OccurredAtUtc)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(x => x.IngestedAtUtc)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()")
                .IsRequired();

            entity.HasIndex(x => new { x.EncounterId, x.OccurredAtUtc });
            entity.HasIndex(x => new { x.PatientId, x.OccurredAtUtc });

            entity.HasIndex(x => x.IdempotencyKey)
                .IsUnique()
                .HasFilter("\"IdempotencyKey\" IS NOT NULL");
        });

        modelBuilder.Entity<PipelineWatermark>(entity =>
        {
            entity.ToTable("pipeline_watermarks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PipelineName).HasMaxLength(100).IsRequired();

            entity.Property(x => x.LastProcessedAtUtc)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()")
                .IsRequired();

            entity.HasIndex(x => x.PipelineName).IsUnique();
        });

        modelBuilder.Entity<FeatureRun>(entity =>
        {
            entity.ToTable("feature_runs");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.PipelineName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EncounterId).IsRequired();

            entity.Property(x => x.WindowStartUtc)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(x => x.WindowEndUtc)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(x => x.FeatureVersion).HasMaxLength(50).IsRequired();
            entity.Property(x => x.InputEventIds).HasColumnType("uuid[]").IsRequired();
            entity.Property(x => x.FeatureHash).HasMaxLength(64).IsRequired();

            entity.Property(x => x.StartedAtUtc)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()")
                .IsRequired();

            entity.Property(x => x.CompletedAtUtc).HasColumnType("timestamptz");
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ProcessedEvents).IsRequired();

            entity.HasIndex(x => new { x.PipelineName, x.EncounterId, x.WindowEndUtc });
        });

        modelBuilder.Entity<PredictionRun>(entity =>
        {
            entity.ToTable("prediction_runs");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.FeatureRunId).IsRequired();
            entity.Property(x => x.ModelVersion).HasMaxLength(100).IsRequired();
            entity.Property(x => x.HorizonHours).IsRequired();
            entity.Property(x => x.RiskScore).IsRequired();
            entity.Property(x => x.SurvivalSummaryJson).HasColumnType("jsonb");
            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()")
                .IsRequired();

            entity.HasIndex(x => new { x.FeatureRunId, x.CreatedAtUtc });

            entity.HasOne(x => x.FeatureRun)
                .WithMany()
                .HasForeignKey(x => x.FeatureRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.EncounterId).IsRequired();
            entity.Property(x => x.PredictionRunId).IsRequired();
            entity.Property(x => x.Threshold).IsRequired();
            entity.Property(x => x.RiskScore).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("now()")
                .IsRequired();
            entity.Property(x => x.AcknowledgedAtUtc).HasColumnType("timestamptz");
            entity.Property(x => x.ClosedAtUtc).HasColumnType("timestamptz");

            entity.HasIndex(x => new { x.EncounterId, x.Status, x.CreatedAtUtc });

            entity.HasOne(x => x.PredictionRun)
                .WithMany()
                .HasForeignKey(x => x.PredictionRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

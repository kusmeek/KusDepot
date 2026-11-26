namespace DataPodServices.DataControl;

public sealed class DataControlDbContext : DbContext
{
    public DataControlDbContext(DbContextOptions<DataControlDbContext> options) : base(options) {}

    public DbSet<DataControlTransferSessionRow> TransferSessions => Set<DataControlTransferSessionRow>();

    public DbSet<DataControlSegmentedRangeRow> SegmentedRanges => Set<DataControlSegmentedRangeRow>();

    public DbSet<DataControlSegmentedCoverageRow> SegmentedCoverage => Set<DataControlSegmentedCoverageRow>();

    public DbSet<DataControlStreamChunkRow> StreamChunks => Set<DataControlStreamChunkRow>();

    public DbSet<DataControlPublicationJournalRow> PublicationJournals => Set<DataControlPublicationJournalRow>();

    public DbSet<DataControlDeletionJobRow> DeletionJobs => Set<DataControlDeletionJobRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataControlTransferSessionRow>(entity =>
        {
            entity.ToTable("TransferSessions");
            entity.HasKey(row => row.SessionID);
            entity.Property(row => row.SessionID).ValueGeneratedNever();
            entity.Property(row => row.CatalogName).IsRequired();
            entity.Property(row => row.ItemInstanceToken).IsRequired();
            entity.Property(row => row.ManifestJson).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.ObjectSHA512).HasColumnType("bytea");
            entity.HasIndex(row => new { row.ItemID , row.Status });
            entity.HasIndex(row => new { row.SourceSessionID , row.CatalogName });
            entity.HasIndex(row => row.Updated);
            entity.HasIndex(row => new { row.CatalogName , row.LastActivity , row.SessionID }).IsUnique(false);
        });

        modelBuilder.Entity<DataControlSegmentedRangeRow>(entity =>
        {
            entity.ToTable("SegmentedRanges");
            entity.HasKey(row => new { row.SessionID , row.Offset });
            entity.Property(row => row.BlobName).IsRequired();
            entity.HasIndex(row => new { row.SessionID , row.Ordinal }).IsUnique();
            entity.HasOne(row => row.Session)
                .WithMany(row => row.SegmentedRanges)
                .HasForeignKey(row => row.SessionID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(row => new { row.SessionID , row.Offset , row.Length });
        });

        modelBuilder.Entity<DataControlSegmentedCoverageRow>(entity =>
        {
            entity.ToTable("SegmentedCoverage",table =>
            {
                table.HasCheckConstraint("CK_SegmentedCoverage_ValidRange","\"Offset\" >= 0 AND \"Length\" > 0 AND \"Offset\" <= 9223372036854775807 - \"Length\"");
            });
            entity.HasKey(row => new { row.SessionID , row.Offset });
            entity.HasOne(row => row.Session)
                .WithMany(row => row.SegmentedCoverage)
                .HasForeignKey(row => row.SessionID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DataControlStreamChunkRow>(entity =>
        {
            entity.ToTable("StreamChunks");
            entity.HasKey(row => new { row.SessionID , row.Offset });
            entity.Property(row => row.BlobName).IsRequired();
            entity.HasIndex(row => new { row.SessionID , row.Ordinal }).IsUnique();
            entity.HasOne(row => row.Session)
                .WithMany(row => row.StreamChunks)
                .HasForeignKey(row => row.SessionID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(row => row.OperationID).IsUnique();
        });

        modelBuilder.Entity<DataControlPublicationJournalRow>(entity =>
        {
            entity.ToTable("PublicationJournals");
            entity.HasKey(row => row.SessionID);
            entity.Property(row => row.CatalogName).IsRequired();
            entity.Property(row => row.ItemInstanceToken).IsRequired();
            entity.Property(row => row.JournalJson).HasColumnType("jsonb").IsRequired();
            entity.Property(row => row.IntegrityDigest).HasColumnType("bytea");
            entity.HasIndex(row => new { row.Phase , row.Updated });
            entity.HasIndex(row => row.OperationID).IsUnique();
        });

        modelBuilder.Entity<DataControlDeletionJobRow>(entity =>
        {
            entity.ToTable("DeletionJobs",table =>
            {
                table.HasCheckConstraint("CK_DeletionJobs_AttemptCount_NonNegative","\"AttemptCount\" >= 0");
            });
            entity.HasKey(row => new { row.CatalogName , row.ItemID , row.ItemInstanceToken });
            entity.Property(row => row.CatalogName).IsRequired();
            entity.Property(row => row.ItemInstanceToken).IsRequired();
            entity.HasIndex(row => new { row.State , row.NextAttempt , row.Updated });
        });
    }
}

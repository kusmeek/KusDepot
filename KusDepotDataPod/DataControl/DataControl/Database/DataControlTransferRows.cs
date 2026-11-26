namespace DataPodServices.DataControl;

public sealed class DataControlTransferSessionRow
{
    public Guid SessionID { get; set; }

    public Guid ItemID { get; set; }

    public Guid ItemInstanceToken { get; set; }

    public String CatalogName { get; set; } = String.Empty;

    public Guid? SourceSessionID { get; set; }

    public Int32 Family { get; set; }

    public Int32 Mode { get; set; }

    public Int32 Status { get; set; }

    public Int64 StateVersion { get; set; }

    public Int64 PlannedBlockCount { get; set; }

    public Int64 CoveredBytes { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public DateTimeOffset LastActivity { get; set; }

    public String ManifestJson { get; set; } = String.Empty;

    public String? ObjectBlobName { get; set; }

    public Int64? ObjectLength { get; set; }

    public Byte[]? ObjectSHA512 { get; set; }

    public ICollection<DataControlSegmentedRangeRow> SegmentedRanges { get; set; } = new List<DataControlSegmentedRangeRow>();

    public ICollection<DataControlSegmentedCoverageRow> SegmentedCoverage { get; set; } = new List<DataControlSegmentedCoverageRow>();

    public ICollection<DataControlStreamChunkRow> StreamChunks { get; set; } = new List<DataControlStreamChunkRow>();
}

public sealed class DataControlSegmentedRangeRow
{
    public Guid SessionID { get; set; }

    public Int64 Offset { get; set; }

    public Int64 Length { get; set; }

    public String BlobName { get; set; } = String.Empty;

    public Byte[] SHA512 { get; set; } = Array.Empty<Byte>();

    public DateTimeOffset Created { get; set; }

    public Int64 Ordinal { get; set; }

    public DataControlTransferSessionRow Session { get; set; } = null!;
}

public sealed class DataControlSegmentedCoverageRow
{
    public Guid SessionID { get; set; }

    public Int64 Offset { get; set; }

    public Int64 Length { get; set; }

    public DataControlTransferSessionRow Session { get; set; } = null!;
}

public sealed class DataControlStreamChunkRow
{
    public Guid SessionID { get; set; }

    public Guid OperationID { get; set; }

    public Int64 Offset { get; set; }

    public Int64 Length { get; set; }

    public String BlobName { get; set; } = String.Empty;

    public Byte[] SHA512 { get; set; } = Array.Empty<Byte>();

    public DateTimeOffset Created { get; set; }

    public Int64 Ordinal { get; set; }

    public DataControlTransferSessionRow Session { get; set; } = null!;
}

public sealed class DataControlPublicationJournalRow
{
    public Guid SessionID { get; set; }

    public Guid ItemID { get; set; }

    public Guid ItemInstanceToken { get; set; }

    public Guid OperationID { get; set; }

    public Int32 Family { get; set; }

    public Int32 Phase { get; set; }

    public Int64 ExpectedStateVersion { get; set; }

    public String CatalogName { get; set; } = String.Empty;

    public Int32 FailureCount { get; set; }

    public String? LastError { get; set; }

    public String? FinalObjectBlobName { get; set; }

    public Int64? FinalObjectLength { get; set; }

    public Byte[]? FinalObjectSHA512 { get; set; }

    public Int64? FinalStreamLength { get; set; }

    public String? IntegrityAlgorithm { get; set; }

    public Byte[]? IntegrityDigest { get; set; }

    public Int64? IntegrityLength { get; set; }

    public Int64? IntegritySegmentCount { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }

    public String JournalJson { get; set; } = String.Empty;
}

public sealed class DataControlDeletionJobRow
{
    public String CatalogName { get; set; } = String.Empty;

    public Guid ItemID { get; set; }

    public Guid ItemInstanceToken { get; set; }

    public Int32 State { get; set; }

    public String? LastError { get; set; }

    public Int32 AttemptCount { get; set; }

    public DateTimeOffset? LastAttempt { get; set; }

    public DateTimeOffset? NextAttempt { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Updated { get; set; }
}
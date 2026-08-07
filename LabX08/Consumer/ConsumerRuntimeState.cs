namespace LabX08;

internal sealed class ConsumerRuntimeState
{
    private const Int32 RecentLineLimit = 18;

    public const String TextOutputMode = "Text";

    public const String HexOutputMode = "Hex";

    public Guid? ItemId { get; set; }

    public Guid? SourceSessionId { get; set; }

    public Guid? LocalSessionId { get; set; }

    public DataControlDownloadMode? Mode { get; set; }

    public Boolean IsStreaming { get; set; }

    public Int64 BytesObserved { get; set; }

    public Int32 ChunksObserved { get; set; }

    public String Status { get; set; } = "Idle";

    public String MaterializedSummary { get; set; } = "None";

    public String OutputMode { get; set; } = TextOutputMode;

    public String FinalItemType { get; set; } = "None";

    public String FinalItemName { get; set; } = "-";

    public String FinalItemDataType { get; set; } = "-";

    public String FinalItemTags { get; set; } = "-";

    public String FinalItemNotes { get; set; } = "-";

    public String ProgressStatus { get; set; } = "No progress yet.";

    public String ProgressRange { get; set; } = "-";

    public String ProgressTotals { get; set; } = "-";

    public List<String> OutputLines { get; init; } = new();

    public void AddOutputLine(String line)
    {
        this.OutputLines.Add(line);

        while(this.OutputLines.Count > RecentLineLimit) { this.OutputLines.RemoveAt(0); }
    }
}

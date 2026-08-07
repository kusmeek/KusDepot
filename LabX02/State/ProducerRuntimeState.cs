namespace LabX02;

internal enum ProducerPhase
{
    Ready,
    Starting,
    Streaming,
    Paused,
    Completing,
    Aborting,
    Completed,
    Aborted,
    Faulted,
}

internal sealed class ProducerRuntimeState
{
    private const Int32 RecentLineLimit = 20;

    public const String BinaryItemType = nameof(BinaryItem);

    public const String TextItemType = nameof(TextItem);

    public const String GenericItemType = nameof(GenericItem);

    public Guid? ItemId { get; set; }

    public Guid? SessionId { get; set; }

    public ProducerPhase Phase { get; set; } = ProducerPhase.Ready;

    public Boolean IsRunning { get; set; }

    public Boolean IsPaused { get; set; }

    public Boolean RandomPauseEnabled { get; set; }

    public Boolean TextMode { get; set; } = true;

    public Int64 BytesProduced { get; set; }

    public Int32 ChunksProduced { get; set; }

    public String Status { get; set; } = "Idle";

    public String FinalItemType { get; set; } = BinaryItemType;

    public String FinalItemDataType { get; set; } = DataTypes.ZIP;

    public String FinalItemName { get; set; } = String.Empty;

    public String FinalItemTagsText { get; set; } = String.Empty;

    public String FinalItemNotesText { get; set; } = String.Empty;

    public String CommittedItemSummary { get; set; } = "None";

    public List<String> RecentLines { get; init; } = new();

    public void AddLine(String line)
    {
        this.RecentLines.Add(line);

        while(this.RecentLines.Count > RecentLineLimit) { this.RecentLines.RemoveAt(0); }
    }

    public ProducerRuntimeState Clone()
    {
        return new ProducerRuntimeState()
        {
            ItemId = this.ItemId,
            SessionId = this.SessionId,
            Phase = this.Phase,
            IsRunning = this.IsRunning,
            IsPaused = this.IsPaused,
            RandomPauseEnabled = this.RandomPauseEnabled,
            TextMode = this.TextMode,
            BytesProduced = this.BytesProduced,
            ChunksProduced = this.ChunksProduced,
            Status = this.Status,
            FinalItemType = this.FinalItemType,
            FinalItemDataType = this.FinalItemDataType,
            FinalItemName = this.FinalItemName,
            FinalItemTagsText = this.FinalItemTagsText,
            FinalItemNotesText = this.FinalItemNotesText,
            CommittedItemSummary = this.CommittedItemSummary,
            RecentLines = new List<String>(this.RecentLines)
        };
    }

    public void SetPhase(ProducerPhase phase , String status)
    {
        this.Phase = phase;
        this.Status = status;
        this.IsRunning = phase is ProducerPhase.Starting
            or ProducerPhase.Streaming
            or ProducerPhase.Paused
            or ProducerPhase.Completing
            or ProducerPhase.Aborting;
        this.IsPaused = phase is ProducerPhase.Paused;
    }
}

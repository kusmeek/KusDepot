namespace LabX08;

internal sealed class ConsumerViewModel : INotifyPropertyChanged
{
    private const Int32 FixedTailLineCount = 200;
    private readonly ConsumerController controller;
    private String status = "Ready";
    private String itemIdText = "-";
    private String sourceSessionIdText = "-";
    private String localSessionIdText = "-";
    private String modeText = "-";
    private String bytesObservedText = "0";
    private String chunksObservedText = "0";
    private String outputModeText = ConsumerRuntimeState.TextOutputMode;
    private String materializedSummary = "None";
    private String finalItemType = "None";
    private String finalItemName = "-";
    private String finalItemDataType = "-";
    private String finalItemTags = "-";
    private String finalItemNotes = "-";
    private String progressStatus = "No progress yet.";
    private String progressRange = "-";
    private String progressTotals = "-";
    private String outputText = String.Empty;
    private String itemIdInput = String.Empty;
    private String sourceSessionIdInput = String.Empty;
    private String localSessionIdInput = String.Empty;

    public ConsumerViewModel(LabDemoOptions options , ConsumerController controller)
    {
        this.Options = options;
        this.controller = controller;
        this.ApplySnapshot(controller.Snapshot());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public LabDemoOptions Options { get; }

    public String Endpoint => this.Options.Endpoint;

    public String WorkingDirectory => this.Options.WorkingDirectory;

    public String Status
    {
        get => this.status;
        private set => this.SetField(ref this.status,value);
    }

    public String ItemIdText
    {
        get => this.itemIdText;
        private set => this.SetField(ref this.itemIdText,value);
    }

    public String SourceSessionIdText
    {
        get => this.sourceSessionIdText;
        private set => this.SetField(ref this.sourceSessionIdText,value);
    }

    public String LocalSessionIdText
    {
        get => this.localSessionIdText;
        private set => this.SetField(ref this.localSessionIdText,value);
    }

    public String ModeText
    {
        get => this.modeText;
        private set => this.SetField(ref this.modeText,value);
    }

    public String BytesObservedText
    {
        get => this.bytesObservedText;
        private set => this.SetField(ref this.bytesObservedText,value);
    }

    public String ChunksObservedText
    {
        get => this.chunksObservedText;
        private set => this.SetField(ref this.chunksObservedText,value);
    }

    public String OutputModeText
    {
        get => this.outputModeText;
        private set => this.SetField(ref this.outputModeText,value);
    }

    public String MaterializedSummary
    {
        get => this.materializedSummary;
        private set => this.SetField(ref this.materializedSummary,value);
    }

    public String FinalItemType
    {
        get => this.finalItemType;
        private set => this.SetField(ref this.finalItemType,value);
    }

    public String FinalItemName
    {
        get => this.finalItemName;
        private set => this.SetField(ref this.finalItemName,value);
    }

    public String FinalItemDataType
    {
        get => this.finalItemDataType;
        private set => this.SetField(ref this.finalItemDataType,value);
    }

    public String FinalItemTags
    {
        get => this.finalItemTags;
        private set => this.SetField(ref this.finalItemTags,value);
    }

    public String FinalItemNotes
    {
        get => this.finalItemNotes;
        private set => this.SetField(ref this.finalItemNotes,value);
    }

    public String ProgressStatus
    {
        get => this.progressStatus;
        private set => this.SetField(ref this.progressStatus,value);
    }

    public String ProgressRange
    {
        get => this.progressRange;
        private set => this.SetField(ref this.progressRange,value);
    }

    public String ProgressTotals
    {
        get => this.progressTotals;
        private set => this.SetField(ref this.progressTotals,value);
    }

    public String OutputText
    {
        get => this.outputText;
        private set => this.SetField(ref this.outputText,value);
    }

    public String ItemIdInput
    {
        get => this.itemIdInput;
        set => this.SetField(ref this.itemIdInput,value);
    }

    public String SourceSessionIdInput
    {
        get => this.sourceSessionIdInput;
        set => this.SetField(ref this.sourceSessionIdInput,value);
    }

    public String LocalSessionIdInput
    {
        get => this.localSessionIdInput;
        set => this.SetField(ref this.localSessionIdInput,value);
    }

    public void ApplySnapshot(ConsumerRuntimeState snapshot)
    {
        this.Status = snapshot.Status;
        this.ItemIdText = FormatGuid(snapshot.ItemId);
        this.SourceSessionIdText = FormatGuid(snapshot.SourceSessionId);
        this.LocalSessionIdText = FormatGuid(snapshot.LocalSessionId);
        this.ModeText = snapshot.Mode?.ToString() ?? "-";
        this.BytesObservedText = snapshot.BytesObserved.ToString("N0",CultureInfo.InvariantCulture);
        this.ChunksObservedText = snapshot.ChunksObserved.ToString("N0",CultureInfo.InvariantCulture);
        this.OutputModeText = snapshot.OutputMode;
        this.MaterializedSummary = snapshot.MaterializedSummary;
        this.FinalItemType = snapshot.FinalItemType;
        this.FinalItemName = snapshot.FinalItemName;
        this.FinalItemDataType = snapshot.FinalItemDataType;
        this.FinalItemTags = snapshot.FinalItemTags;
        this.FinalItemNotes = snapshot.FinalItemNotes;
        this.ProgressStatus = snapshot.ProgressStatus;
        this.ProgressRange = snapshot.ProgressRange;
        this.ProgressTotals = snapshot.ProgressTotals;
        this.OutputText = String.Join(Environment.NewLine,snapshot.OutputLines.TakeLast(FixedTailLineCount));
    }

    public Task OpenLiveAsync()
    {
        return this.controller.OpenLiveFollowAsync(
            ParseGuid(this.ItemIdInput,"Item ID"),
            ParseGuid(this.SourceSessionIdInput,"Source Session ID"));
    }

    public Task ResumeAsync()
    {
        return this.controller.ResumeLocalAsync(ParseGuid(this.LocalSessionIdInput,"Local Session ID"));
    }

    public Task MaterializeAsync() => this.controller.MaterializeAsync();

    public Task StopAsync() => this.controller.StopAsync();

    public void ToggleOutputMode() => this.controller.ToggleOutputMode();

    public void ClearOutput() => this.controller.ClearOutput();

    public void ResetDisplay()
    {
        this.LocalSessionIdInput = String.Empty;
        this.controller.ResetDisplay();
    }

    private static Guid ParseGuid(String? text , String fieldName)
    {
        if(Guid.TryParse(text?.Trim(),out Guid value)) { return value; }

        throw new InvalidOperationException($"{fieldName} must contain a valid GUID.");
    }

    private static String FormatGuid(Guid? value)
    {
        return value.HasValue ? value.Value.ToString() : "-";
    }

    private Boolean SetField<T>(ref T field , T value , String? propertyName = null)
    {
        if(EqualityComparer<T>.Default.Equals(field,value)) { return false; }
        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] String? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
    }
}

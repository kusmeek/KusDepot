namespace LabX02;

internal sealed class ProducerViewModel : INotifyPropertyChanged
{
    private readonly ProducerController controller;
    private Boolean suppressMetadataPush;
    private String status = "Ready";
    private String itemIdText = "-";
    private String sessionIdText = "-";
    private String modeText = "Text";
    private String bytesProducedText = "0";
    private String chunksProducedText = "0";
    private String committedItemSummary = "None";
    private String pauseButtonText = "Pause";
    private String randomPauseButtonText = "Random Pause Off";
    private Boolean canTogglePayloadMode = true;
    private Boolean isRunning;
    private String finalItemType = ProducerRuntimeState.BinaryItemType;
    private IReadOnlyList<String> availableFinalItemDataTypes = Array.Empty<String>();
    private String? selectedFinalItemDataType;
    private String finalItemDataTypeText = String.Empty;
    private Boolean isGenericDataType;
    private String finalItemName = "LabX02 Final Item";
    private String finalItemTagsText = String.Empty;
    private String finalItemNotesText = String.Empty;
    private String logText = String.Empty;
    private Boolean metadataInitialized;

    public ProducerViewModel(LabDemoOptions options , ProducerController controller)
    {
        this.Options = options;
        this.controller = controller;
        ProducerRuntimeState snapshot = controller.Snapshot();
        this.ApplyMetadataSnapshot(snapshot);
        this.ApplySnapshot(snapshot);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public LabDemoOptions Options { get; }

    public IReadOnlyList<String> FinalItemTypes { get; } = new[]
    {
        ProducerRuntimeState.BinaryItemType,
        ProducerRuntimeState.TextItemType,
        ProducerRuntimeState.GenericItemType,
    };

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

    public String SessionIdText
    {
        get => this.sessionIdText;
        private set => this.SetField(ref this.sessionIdText,value);
    }

    public String ModeText
    {
        get => this.modeText;
        private set => this.SetField(ref this.modeText,value);
    }

    public String BytesProducedText
    {
        get => this.bytesProducedText;
        private set => this.SetField(ref this.bytesProducedText,value);
    }

    public String ChunksProducedText
    {
        get => this.chunksProducedText;
        private set => this.SetField(ref this.chunksProducedText,value);
    }

    public String CommittedItemSummary
    {
        get => this.committedItemSummary;
        private set => this.SetField(ref this.committedItemSummary,value);
    }

    public String PauseButtonText
    {
        get => this.pauseButtonText;
        private set => this.SetField(ref this.pauseButtonText,value);
    }

    public String RandomPauseButtonText
    {
        get => this.randomPauseButtonText;
        private set => this.SetField(ref this.randomPauseButtonText,value);
    }

    public Boolean CanTogglePayloadMode
    {
        get => this.canTogglePayloadMode;
        private set => this.SetField(ref this.canTogglePayloadMode,value);
    }

    public Boolean IsRunning
    {
        get => this.isRunning;
        private set => this.SetField(ref this.isRunning,value);
    }

    public String FinalItemType
    {
        get => this.finalItemType;
        set
        {
            if(this.SetField(ref this.finalItemType,value))
            {
                this.RefreshDataTypeChoices(preferredType:this.CurrentFinalItemDataType);
                this.PushMetadataToController();
            }
        }
    }

    public IReadOnlyList<String> AvailableFinalItemDataTypes
    {
        get => this.availableFinalItemDataTypes;
        private set => this.SetField(ref this.availableFinalItemDataTypes,value);
    }

    public String? SelectedFinalItemDataType
    {
        get => this.selectedFinalItemDataType;
        set
        {
            if(this.SetField(ref this.selectedFinalItemDataType,value))
            {
                this.PushMetadataToController();
            }
        }
    }

    public String FinalItemDataTypeText
    {
        get => this.finalItemDataTypeText;
        set
        {
            if(this.SetField(ref this.finalItemDataTypeText,value))
            {
                this.PushMetadataToController();
            }
        }
    }

    public Boolean IsGenericDataType
    {
        get => this.isGenericDataType;
        private set
        {
            if(this.SetField(ref this.isGenericDataType,value))
            {
                this.OnPropertyChanged(nameof(this.DataTypeComboBoxVisibility));
                this.OnPropertyChanged(nameof(this.DataTypeTextBoxVisibility));
            }
        }
    }

    public Visibility DataTypeComboBoxVisibility => this.IsGenericDataType
        ? Visibility.Collapsed
        : Visibility.Visible;

    public Visibility DataTypeTextBoxVisibility => this.IsGenericDataType
        ? Visibility.Visible
        : Visibility.Collapsed;

    public String FinalItemName
    {
        get => this.finalItemName;
        set
        {
            if(this.SetField(ref this.finalItemName,value))
            {
                this.PushMetadataToController();
            }
        }
    }

    public String FinalItemTagsText
    {
        get => this.finalItemTagsText;
        set
        {
            if(this.SetField(ref this.finalItemTagsText,value))
            {
                this.PushMetadataToController();
            }
        }
    }

    public String FinalItemNotesText
    {
        get => this.finalItemNotesText;
        set
        {
            if(this.SetField(ref this.finalItemNotesText,value))
            {
                this.PushMetadataToController();
            }
        }
    }

    public String LogText
    {
        get => this.logText;
        private set => this.SetField(ref this.logText,value);
    }

    public void ApplySnapshot(ProducerRuntimeState snapshot)
    {
        this.suppressMetadataPush = true;

        try
        {
            this.Status = snapshot.Status;
            this.ItemIdText = FormatGuid(snapshot.ItemId);
            this.SessionIdText = FormatGuid(snapshot.SessionId);
            this.ModeText = snapshot.TextMode ? "Text" : "Bytes";
            this.BytesProducedText = snapshot.BytesProduced.ToString("N0",CultureInfo.InvariantCulture);
            this.ChunksProducedText = snapshot.ChunksProduced.ToString("N0",CultureInfo.InvariantCulture);
            this.CommittedItemSummary = snapshot.CommittedItemSummary;
            this.PauseButtonText = snapshot.IsPaused ? "Resume" : "Pause";
            this.RandomPauseButtonText = snapshot.RandomPauseEnabled ? "Random Pause On" : "Random Pause Off";
            this.CanTogglePayloadMode = snapshot.IsRunning is false;
            this.IsRunning = snapshot.IsRunning;
            this.LogText = String.Join(Environment.NewLine,snapshot.RecentLines);

            if(this.metadataInitialized is false)
            {
                this.ApplyMetadataSnapshot(snapshot);
            }
        }
        finally
        {
            this.suppressMetadataPush = false;
        }
    }

    public Task StartAsync() => this.controller.StartLiveAsync();

    public Task CompleteAsync() => this.controller.CompleteLiveAsync();

    public Task AbortAsync() => this.controller.AbortLiveAsync();

    public Task StopAsync() => this.controller.StopAsync();

    public void TogglePause() => this.controller.TogglePause();

    public void ToggleRandomPause() => this.controller.ToggleRandomPause();

    public void TogglePayloadMode() => this.controller.TogglePayloadMode();

    public void ResetDisplay() => this.controller.ResetDisplay();

    public void ResetMetadataFromController()
    {
        this.ApplyMetadataSnapshot(this.controller.Snapshot());
    }

    private String? CurrentFinalItemDataType => this.IsGenericDataType ? this.FinalItemDataTypeText : this.SelectedFinalItemDataType;

    private void ApplyMetadataSnapshot(ProducerRuntimeState snapshot)
    {
        this.finalItemType = snapshot.FinalItemType;
        this.OnPropertyChanged(nameof(this.FinalItemType));
        this.finalItemName = snapshot.FinalItemName;
        this.OnPropertyChanged(nameof(this.FinalItemName));
        this.finalItemTagsText = snapshot.FinalItemTagsText;
        this.OnPropertyChanged(nameof(this.FinalItemTagsText));
        this.finalItemNotesText = snapshot.FinalItemNotesText;
        this.OnPropertyChanged(nameof(this.FinalItemNotesText));
        this.RefreshDataTypeChoices(snapshot.FinalItemDataType);
        this.metadataInitialized = true;
    }

    private void RefreshDataTypeChoices(String? preferredType)
    {
        if(String.Equals(this.FinalItemType,ProducerRuntimeState.GenericItemType,StringComparison.Ordinal))
        {
            this.IsGenericDataType = true;
            this.AvailableFinalItemDataTypes = Array.Empty<String>();
            this.selectedFinalItemDataType = null;
            this.OnPropertyChanged(nameof(this.SelectedFinalItemDataType));
            this.finalItemDataTypeText = preferredType ?? String.Empty;
            this.OnPropertyChanged(nameof(this.FinalItemDataTypeText));
            return;
        }

        this.IsGenericDataType = false;

        String[] values = ProducerController.GetAvailableFinalItemDataTypes(this.FinalItemType)
            .Where(_ => String.IsNullOrWhiteSpace(_) is false)
            .Cast<String>()
            .ToArray();

        this.AvailableFinalItemDataTypes = values;

        String resolved = values.FirstOrDefault(_ => String.Equals(_,preferredType,StringComparison.OrdinalIgnoreCase))
            ?? values.FirstOrDefault()
            ?? DataTypes.NONE;

        this.selectedFinalItemDataType = resolved;
        this.OnPropertyChanged(nameof(this.SelectedFinalItemDataType));
        this.finalItemDataTypeText = resolved;
        this.OnPropertyChanged(nameof(this.FinalItemDataTypeText));
    }

    private void PushMetadataToController()
    {
        if(this.suppressMetadataPush) { return; }

        this.controller.UpdateFinalItemMetadata(
            this.FinalItemType,
            this.CurrentFinalItemDataType,
            this.FinalItemName,
            SplitLines(this.FinalItemTagsText),
            SplitLines(this.FinalItemNotesText));
    }

    private static String[] SplitLines(String? text)
    {
        return (text ?? String.Empty)
            .Replace("\r\n","\n",StringComparison.Ordinal)
            .Split('\n',StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

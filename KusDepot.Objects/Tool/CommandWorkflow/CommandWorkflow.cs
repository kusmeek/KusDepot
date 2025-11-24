namespace KusDepot;

/**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.CommandWorkflow")]
[DataContract(Name = "CommandWorkflow" , Namespace = "KusDepot")]
public sealed class CommandWorkflow : IParsable<CommandWorkflow>
{
    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="Details"]/*'/>*/
    [IgnoreDataMember] [JsonIgnore] [Id(2)]
    public CommandDetails?            Details  { get { return this.details;  } set { this.details  ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="EventLog"]/*'/>*/
    [DataMember(Name = "EventLog" , EmitDefaultValue = true , IsRequired = true)] [JsonPropertyOrder(1)] [Id(1)]
    public Dictionary<String,String>? EventLog { get { return this.eventlog; } set { this.eventlog ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="Sequence"]/*'/>*/
    [DataMember(Name = "Sequence" , EmitDefaultValue = true , IsRequired = true)] [JsonPropertyOrder(0)] [Id(0)]
    public SortedList<Int32,String>?  Sequence { get { return this.sequence; } set { this.sequence ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="details"]/*'/>*/
    [NonSerialized]
    private CommandDetails?            details;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="eventlog"]/*'/>*/
    [NonSerialized]
    private Dictionary<String,String>? eventlog;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="sequence"]/*'/>*/
    [NonSerialized]
    private SortedList<Int32,String>?  sequence;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/constructor[@name="ConstructorJson"]/*'/>*/
    [JsonConstructor]
    private CommandWorkflow() {}

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/constructor[@name="Constructor"]/*'/>*/
    public CommandWorkflow(CommandDetails? details = null)
    {
        try { this.sequence = new(); this.eventlog = new(); this.details = details; } catch {}
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="LogEvent"]/*'/>*/
    public Boolean LogEvent(String? action = null , String? data = null)
    {
        try
        {
            if(this.eventlog is null) { this.eventlog = new(); } data ??= String.Empty;

            String index = CommandWorkflowEvent.CreateEventLogIndex(this.details!,action);

            var log = this.details!.GetArgument("Logger") as ILogger;

            log?.Information(CommandWorkflowLog,index,data);

            this.eventlog[index] = data;

            return true;
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    static CommandWorkflow IParsable<CommandWorkflow>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="Parse"]/*'/>*/
    public static CommandWorkflow? Parse(String input)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; }

            return JsonSerializer.Deserialize<CommandWorkflow>(input,new JsonSerializerOptions()
                { PropertyNameCaseInsensitive = false , AllowTrailingCommas = true });
        }
        catch { return null; }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        try { return JsonSerializer.Serialize(this,new JsonSerializerOptions(){ WriteIndented = true }); }

        catch { return String.Empty; }
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out CommandWorkflow result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/main/*'/>*/
public sealed class CommandWorkflowEvent : IEquatable<CommandWorkflowEvent> , IParsable<CommandWorkflowEvent>
{
    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Action"]/*'/>*/
    public String Action { get; init; } = String.Empty;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Data"]/*'/>*/
    public String? Data { get => field; set { field ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="DetailsID"]/*'/>*/
    public Guid DetailsID { get; init; }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Handle"]/*'/>*/
    public String Handle { get; init; } = String.Empty;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Time"]/*'/>*/
    public DateTimeOffset Time { get; init; }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="ToolID"]/*'/>*/
    public Guid ToolID { get; init; }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="CreateEventLogIndex"]/*'/>*/
    public static String CreateEventLogIndex(CommandDetails details , String? action)
    {
        try
        {
            if(details is null || String.IsNullOrEmpty(action)) { return String.Empty; }

            String? id = details.ID!.Value.ToString("N").ToUpperInvariant(); if(String.IsNullOrEmpty(id)) { return String.Empty; }

            String? toolid = (details.GetArgument("AttachedToolID") as Guid?)?.ToString("N").ToUpperInvariant(); if(String.IsNullOrEmpty(toolid)) { return String.Empty; }

            String handle = details.Handle ?? String.Empty; if(String.IsNullOrEmpty(handle)) { return String.Empty; }

            String time = DateTimeOffset.Now.ToString("O",CultureInfo.InvariantCulture);

            return String.Concat(id,"~",toolid,"~",handle,"~",action,"~",time);
        }
        catch { return String.Empty; }
    }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other)
    {
        return this.Equals(other as CommandWorkflowEvent);
    }

    ///<inheritdoc/>
    public Boolean Equals(CommandWorkflowEvent? other)
    {
        if(other is null) { return false; }

        if(ReferenceEquals(this,other)) { return true; }

        return this.Time.Equals(other.Time);
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(CommandWorkflowEvent? a , CommandWorkflowEvent? b)
    {
        if(a is null) { return b is null; } return a.Equals(b);
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(CommandWorkflowEvent? a , CommandWorkflowEvent? b)
    {
        return !(a == b);
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return this.Time.GetHashCode(); }

    ///<inheritdoc/>
    static CommandWorkflowEvent IParsable<CommandWorkflowEvent>.Parse(String input, IFormatProvider? format)
    {
        return Parse(input);
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="Parse"]/*'/>*/
    public static CommandWorkflowEvent Parse(String? input)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return new(); }

            String[] parts = input.Split('~'); if(parts.Length != 5) { throw new FormatException("Invalid input format"); }

            if(Guid.TryParseExact(parts[0],"N",out Guid id) is false) { throw new FormatException("Invalid DetailsID"); }

            if(Guid.TryParseExact(parts[1],"N",out Guid tid) is false) { throw new FormatException("Invalid ToolID"); }

            String handle = parts[2];

            String action = parts[3];

            if(DateTimeOffset.TryParseExact(parts[4],"O",CultureInfo.InvariantCulture,DateTimeStyles.None,out var time) is false)
            { throw new FormatException("Invalid timestamp"); }

            return new CommandWorkflowEvent{ DetailsID = id , ToolID = tid , Handle = handle , Action = action , Time = time };
        }
        catch ( Exception _ ) { KusDepotLog.Verbose(_,ParseFail); return new CommandWorkflowEvent(); }
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="ParseLine"]/*'/>*/
    public static CommandWorkflowEvent ParseLine(String? line)
    {
        try
        {
            if(String.IsNullOrEmpty(line)) { return new(); }

            Int32 idx = line.LastIndexOf("] ",Ordinal); if(idx < 0 || idx + 2 >= line.Length) { return new(); }

            String rest = line[(idx + 2)..]; Int32 spc = rest.IndexOf(' '); String key = spc >= 0 ? rest[..spc] : rest;

            String data = spc >= 0 && spc + 1 <= rest.Length ? rest[(spc + 1)..] : String.Empty;

            var evt = Parse(key); evt.Data = data; return evt;
        }
        catch ( Exception _ ) { KusDepotLog.Verbose(_,ParseLineFail); return new CommandWorkflowEvent(); }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        try
        {
            String id = this.DetailsID.ToString("N").ToUpperInvariant();

            String tool = this.ToolID.ToString("N").ToUpperInvariant();

            String handle = this.Handle ?? String.Empty;

            String action = this.Action ?? String.Empty;

            String time = this.Time.ToString("O", CultureInfo.InvariantCulture);

            return String.Concat(id, "~", tool, "~", handle, "~", action, "~", time);
        }
        catch { return String.Empty; }
    }

    ///<inheritdoc/>
    public static Boolean TryParse(String? input, IFormatProvider? format, out CommandWorkflowEvent result)
    {
        result = new CommandWorkflowEvent(); if(String.IsNullOrEmpty(input)) { return false; }

        var parts = input.Split('~'); if(parts.Length != 5) { return false; }

        if(!Guid.TryParseExact(parts[0],"N",out Guid id)) { return false; }

        if(!Guid.TryParseExact(parts[1],"N",out Guid tid)) { return false; }

        var handle = parts[2];

        var action = parts[3];

        if(!DateTimeOffset.TryParseExact(parts[4],"O",CultureInfo.InvariantCulture,DateTimeStyles.None,out var time)) { return false; }

        result = new CommandWorkflowEvent { DetailsID = id , ToolID = tid , Handle = handle , Action = action , Time = time }; return true;
    }
}
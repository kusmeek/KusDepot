namespace KusDepot;

/**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.CommandWorkflow")]
[DataContract(Name = "CommandWorkflow" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed class CommandWorkflow : IParsable<CommandWorkflow>
{
    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="Details"]/*'/>*/
    [IgnoreDataMember] [JsonIgnore] [Id(3)]
    public CommandDetails?            Details  { get { return this.details;  } set { this.details  ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="EventLog"]/*'/>*/
    [JsonPropertyName("EventLog")] [JsonRequired] [JsonPropertyOrder(2)] [Id(2)]
    [DataMember(Name = "EventLog" , EmitDefaultValue = true , IsRequired = true , Order = 2)]
    public Dictionary<String,String>? EventLog { get { return this.eventlog; } set { this.eventlog ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonRequired] [JsonPropertyOrder(0)] [Id(0)]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true , Order = 0)]
    public String?                    Name     { get { return this.name;     } set { this.name ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/property[@name="Sequence"]/*'/>*/
    [JsonPropertyName("Sequence")] [JsonRequired] [JsonPropertyOrder(1)] [Id(1)]
    [DataMember(Name = "Sequence" , EmitDefaultValue = true , IsRequired = true , Order = 1)]
    public SortedList<Int32,String>?  Sequence { get { return this.sequence; } set { this.sequence ??= value; } }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="details"]/*'/>*/
    [NonSerialized]
    private CommandDetails?            details;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="eventlog"]/*'/>*/
    [NonSerialized]
    private Dictionary<String,String>? eventlog;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="name"]/*'/>*/
    [NonSerialized]
    private String?                    name;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/field[@name="sequence"]/*'/>*/
    [NonSerialized]
    private SortedList<Int32,String>?  sequence;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/constructor[@name="ConstructorJson"]/*'/>*/
    [JsonConstructor]
    internal CommandWorkflow() {}

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/constructor[@name="Constructor"]/*'/>*/
    public CommandWorkflow(CommandDetails? details = null , String? name = null)
    {
        try { this.sequence = new(); this.eventlog = new(); this.details = details; this.name = name; } catch {}
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="LogEvent"]/*'/>*/
    public Boolean LogEvent(String? action = null , String? data = null)
    {
        String index = CommandWorkflowEvent.CreateEventLogIndex(this.details!,action);

        this.details?.GetLogger()?.Information(CommandWorkflowLog,index,data ?? String.Empty);

        if(this.eventlog is null) { this.eventlog = new(); }

        this.eventlog[index] = data ?? String.Empty;

        return true;
    }

    ///<inheritdoc/>
    static CommandWorkflow IParsable<CommandWorkflow>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="Parse"]/*'/>*/
    public static CommandWorkflow? Parse(String input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        return JsonUtility.Parse<CommandWorkflow>(input);
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        return JsonUtility.ToJsonString<CommandWorkflow>(this);
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflow"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out CommandWorkflow result)
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch { result = null!; return false; }
    }
}

/**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/main/*'/>*/
public sealed class CommandWorkflowEvent : IEquatable<CommandWorkflowEvent> , IParsable<CommandWorkflowEvent>
{
    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Action"]/*'/>*/
    public String Action { get; init; } = String.Empty;

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/property[@name="Data"]/*'/>*/
    public String? Data { get; set { field ??= value; } }

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
        ArgumentNullException.ThrowIfNull(details); ArgumentException.ThrowIfNullOrEmpty(action);

        if(details.ID is null || details.ID == Guid.Empty) { throw new InvalidOperationException("Command details must have a non-empty ID."); }

        Guid? toolid = details.GetArgument<Guid?>("!AttachedToolID");

        if(toolid is null || toolid == Guid.Empty) { throw new InvalidOperationException("Command details must have an attached tool ID."); }

        String handle = details.Handle ?? String.Empty;

        if(String.IsNullOrEmpty(handle)) { throw new InvalidOperationException("Command details must have a handle."); }

        String time = DateTimeOffset.Now.ToString("O",CultureInfo.InvariantCulture);

        return String.Concat(details.ID.Value.ToString("N").ToUpperInvariant(),"~",toolid.Value.ToString("N").ToUpperInvariant(),"~",handle,"~",action,"~",time);
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
        if(String.IsNullOrEmpty(input)) { throw new FormatException("Input must contain an event log index."); }

        String[] parts = input.Split('~'); if(parts.Length != 5) { throw new FormatException("Invalid input format"); }

        if(Guid.TryParseExact(parts[0],"N",out Guid id) is false) { throw new FormatException("Invalid DetailsID"); }

        if(Guid.TryParseExact(parts[1],"N",out Guid tid) is false) { throw new FormatException("Invalid ToolID"); }

        String handle = parts[2];

        String action = parts[3];

        if(DateTimeOffset.TryParseExact(parts[4],"O",CultureInfo.InvariantCulture,DateTimeStyles.None,out var time) is false)
        { throw new FormatException("Invalid timestamp"); }

        return new CommandWorkflowEvent{ DetailsID = id , ToolID = tid , Handle = handle , Action = action , Time = time };
    }

    /**<include file='CommandWorkflow.xml' path='CommandWorkflow/class[@name="CommandWorkflowEvent"]/method[@name="ParseLine"]/*'/>*/
    public static CommandWorkflowEvent ParseLine(String? line)
    {
        if(String.IsNullOrEmpty(line)) { throw new FormatException("Input must contain a log line."); }

        Int32 idx = line.LastIndexOf("] ",Ordinal); if(idx < 0 || idx + 2 >= line.Length) { throw new FormatException("Invalid log line format."); }

        String rest = line[(idx + 2)..]; Int32 spc = rest.IndexOf(' '); String key = spc >= 0 ? rest[..spc] : rest;

        String data = spc >= 0 && spc + 1 <= rest.Length ? rest[(spc + 1)..] : String.Empty;

        var evt = Parse(key); evt.Data = data; return evt;
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        String id = this.DetailsID.ToString("N").ToUpperInvariant();

        String tool = this.ToolID.ToString("N").ToUpperInvariant();

        String handle = this.Handle ?? String.Empty;

        String action = this.Action ?? String.Empty;

        String time = this.Time.ToString("O", CultureInfo.InvariantCulture);

        return String.Concat(id, "~", tool, "~", handle, "~", action, "~", time);
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
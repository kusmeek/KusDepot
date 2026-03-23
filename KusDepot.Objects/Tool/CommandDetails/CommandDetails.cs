namespace KusDepot;

/**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.CommandDetails")]
[DataContract(Name = "CommandDetails" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed class CommandDetails : IParsable<CommandDetails>
{
    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/field[@name="arguments"]/*'/>*/
    [NonSerialized]
    private Dictionary<String,Object?>? arguments;

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/field[@name="handle"]/*'/>*/
    [NonSerialized]
    private String?                     handle;

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/field[@name="id"]/*'/>*/
    [NonSerialized]
    private Guid?                       id;

    [NonSerialized]
    private CommandWorkflow?            workflow;

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="Arguments"]/*'/>*/
    [DataMember(Name = "Arguments" , EmitDefaultValue = true , IsRequired = true)] [JsonIgnore] [Id(0)]
    public Dictionary<String,Object?>? Arguments { get { return this.arguments; } set { this.arguments ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="MyArguments"]/*'/>*/
    [JsonPropertyName("Arguments")] [JsonRequired] [IgnoreDataMember]
    public Dictionary<String,ToolValue?>? MyArguments
    {
        get { try { return ToolValueConverter.ToToolValues(this.arguments); } catch { return null; } }

        set { try { if(this.arguments is null) { this.arguments = ToolValueConverter.FromToolValues(value); } } catch {} }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="Handle"]/*'/>*/
    [JsonPropertyName("Handle")] [JsonRequired] [Id(1)]
    [DataMember(Name = "Handle" , EmitDefaultValue = true , IsRequired = true)]
    public String?                     Handle    { get { return this.handle;    } set { this.handle    ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired] [Id(2)]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid?                       ID        { get { return this.id;        } set { this.id        ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="Workflow"]/*'/>*/
    [JsonPropertyName("Workflow")] [JsonRequired] [Id(3)]
    [DataMember(Name = "Workflow" , EmitDefaultValue = true , IsRequired = true)]
    public CommandWorkflow?            Workflow
    {
        get { return this.workflow; }

        set { if(this.workflow is null) { this.workflow = value; if(this.workflow is not null) { this.workflow.Details ??= this; } } }
    }

    [JsonConstructor]
    private CommandDetails() {}

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/constructor[@name="Constructor"]/*'/>*/
    public CommandDetails(Dictionary<String,Object?>? arguments = null , String? handle = null , Guid? id = null , CommandWorkflow? workflow = null)
    {
        Arguments = arguments; Handle = handle; ID = id; Workflow = workflow;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="Create"]/*'/>*/
    public static CommandDetails Create(String? handle = null , Dictionary<String,Object?>? arguments = null , Guid? id = null , CommandWorkflow? workflow = null) => new(arguments,handle,id,workflow);

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="MakeReady"]/*'/>*/
    public Boolean MakeReady(ITool tool)
    {
        try
        {
            if(String.IsNullOrEmpty(this.handle)) { return false; }

            if( this.id is null || Equals(this.id,Guid.Empty) ) { this.id = Guid.NewGuid(); }

            if(this.arguments is null) { this.arguments = new Dictionary<String,Object?>(); }

            this.SetAttachedToolID(tool?.GetID());

            return true;
        }
        catch { return false; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetAccessKey"]/*'/>*/
    public AccessKey? GetAccessKey() { return this.GetArgument<AccessKey?>("AccessKey"); }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetArgument"]/*'/>*/
    public Object? GetArgument(String? name)
    {
        try { return String.IsNullOrEmpty(name) is false && this.Arguments?.ContainsKey(name) is true ? this.Arguments[name!] : null; } catch { return null; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetArgumentType"]/*'/>*/
    public TResult? GetArgument<TResult>(String? name)
    {
        try { return String.IsNullOrEmpty(name) is false && this.Arguments?.ContainsKey(name) is true ? (TResult?)this.Arguments[name!] : default; } catch { return default; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetCancel"]/*'/>*/
    public CancellationToken GetCancel() { return this.GetArgument<CancellationToken?>("Cancel") ?? CancellationToken.None; }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetLogger"]/*'/>*/
    public ILogger? GetLogger() { return this.GetArgument<ILogger?>("Logger"); }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetTimeout"]/*'/>*/
    public TimeSpan? GetTimeout() { return this.GetArgument<TimeSpan?>("Timeout"); }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="GetToolCancel"]/*'/>*/
    public CancellationToken GetToolCancel()
    {
        try { return this.GetArgument<CancellationToken?>("!ToolCancel") ?? CancellationToken.None; }

        catch { return CancellationToken.None; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="PreserveActivity"]/*'/>*/
    public CommandDetails PreserveActivity(Boolean preserve)
    {
        try { this.arguments ??= new Dictionary<String,Object?>(); this.arguments["!PreserveActivity"] = preserve; }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetAccessKey"]/*'/>*/
    public CommandDetails SetAccessKey(AccessKey? key = null)
    {
        try { if(key is not null) { this.SetArgument("AccessKey",key,true); } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetArgument"]/*'/>*/
    public CommandDetails SetArgument(String? name = null , Object? argument = null , Boolean overwrite = false)
    {
        try
        {
            if(String.IsNullOrEmpty(name) || name.AsSpan().StartsWith("!")) { return this; }

            this.arguments ??= new Dictionary<String,Object?>();

            if(this.arguments.ContainsKey(name) is false || overwrite is true) { this.arguments[name] = argument; }
        }
        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetAttachedToolID"]/*'/>*/
    public CommandDetails SetAttachedToolID(Guid? id)
    {
        try { this.arguments ??= new Dictionary<String,Object?>(); this.arguments["!AttachedToolID"] = id; }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetCancel"]/*'/>*/
    public CommandDetails SetCancel(CancellationToken? cancel = null)
    {
        try { if(cancel is not null) { this.SetArgument("Cancel",cancel); } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetFreeMode"]/*'/>*/
    public CommandDetails SetFreeMode(Boolean freemode)
    {
        try { this.SetArgument("FreeMode",freemode,true); }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetHandle"]/*'/>*/
    public CommandDetails SetHandle(String? handle = null)
    {
        try { if(handle is not null) { this.Handle = handle; } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetID"]/*'/>*/
    public CommandDetails SetID(Guid? id = null)
    {
        try { if(id is not null) { this.ID = id; } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetInputStream"]/*'/>*/
    public CommandDetails SetInputStream(Stream? stream = null)
    {
        try { if(stream is not null) { this.SetArgument("InputStream",stream,true); } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetLogger"]/*'/>*/
    public CommandDetails SetLogger(ILogger? logger = null)
    {
        try { if(logger is not null) { this.SetArgument("Logger",logger); } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetTimeout"]/*'/>*/
    public CommandDetails SetTimeout(TimeSpan? timeout = null)
    {
        try { if(timeout is not null) { this.SetArgument("Timeout",timeout); } }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetToolCancel"]/*'/>*/
    public CommandDetails SetToolCancel(CancellationToken? cancel = null)
    {
        try { this.arguments ??= new Dictionary<String,Object?>(); this.arguments["!ToolCancel"] = cancel ?? CancellationToken.None; }

        catch {} return this;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="SetWorkflow"]/*'/>*/
    public CommandDetails SetWorkflow(CommandWorkflow? workflow = null)
    {
        try { if(workflow is not null) { this.Workflow = workflow; } }

        catch {} return this;
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        return DataContractUtility.ToBase64String(this,SerializationData.ForType(this.GetType()));
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { if(this.workflow is not null) { this.workflow.Details ??= this; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="IParsable{CommandDetails}.Parse"]/*'/>*/
    static CommandDetails IParsable<CommandDetails>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="Parse"]/*'/>*/
    public static CommandDetails? Parse(String input , IFormatProvider? format = null)
    {
        return DataContractUtility.ParseBase64<CommandDetails>(input,SerializationData.ForType(typeof(CommandDetails)));
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out CommandDetails details)
    {
        return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(CommandDetails)),out details);
    }
}
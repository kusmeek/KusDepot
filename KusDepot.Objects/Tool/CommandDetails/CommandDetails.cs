namespace KusDepot;

/**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.CommandDetails")]
[DataContract(Name = "CommandDetails" , Namespace = "KusDepot")]
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
    [DataMember(Name = "Arguments" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Dictionary<String,Object?>? Arguments { get { return this.arguments; } set { this.arguments ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="Handle"]/*'/>*/
    [DataMember(Name = "Handle" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String?                     Handle    { get { return this.handle;    } set { this.handle    ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public Guid?                       ID        { get { return this.id;        } set { this.id        ??= value; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/property[@name="Workflow"]/*'/>*/
    [DataMember(Name = "Workflow" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public CommandWorkflow?            Workflow 
    {
        get { return this.workflow; }

        set { if(this.workflow is null) { this.workflow = value; if(this.workflow is not null) { this.workflow.Details ??= this; } } }
    }

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

            this.SetArgument("AttachedToolID",tool?.GetID());

            return true;
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(CommandDetails));

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch { return String.Empty; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { if(this.workflow is not null) { this.workflow.Details ??= this; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="IParsable{CommandDetails}.Parse"]/*'/>*/
    static CommandDetails IParsable<CommandDetails>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="Parse"]/*'/>*/
    public static CommandDetails? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(CommandDetails)); CommandDetails? _2 = _1.ReadObject(_0) as CommandDetails; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch { return null; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetails"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out CommandDetails key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(CommandDetails)); CommandDetails? _2 = _1.ReadObject(_0) as CommandDetails; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/main/*'/>*/
[DataContract(Name = "CommandDetailsWeb" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("KusDepot.CommandDetailsWeb")]
public record CommandDetailsWeb
{
    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/property[@name="Arguments"]/*'/>*/
    [DataMember(Name = "Arguments" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Dictionary<String,String?>? Arguments { get; init; } = new();

    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/property[@name="Handle"]/*'/>*/
    [DataMember(Name = "Handle" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Handle { get; set; }

    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public Guid? ID { get; set; }

    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/property[@name="Workflow"]/*'/>*/
    [DataMember(Name = "Workflow" , EmitDefaultValue = true , IsRequired = false)] [Id(3)]
    public CommandWorkflow? Workflow { get; set; }

    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/method[@name="ToCommandDetails"]/*'/>*/
    public virtual CommandDetails ToCommandDetails()
    {
        return new CommandDetails(Arguments?.ToDictionary(_ => _.Key,_ => (Object?)_.Value),this.Handle,this.ID,this.Workflow);
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/record[@name="CommandDetailsWeb"]/method[@name="Parse"]/*'/>*/
    public static CommandDetailsWeb? Parse(String input) { try { return String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<CommandDetailsWeb>(input); } catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; } }

    ///<inheritdoc/>
    public override String ToString() { try { return JsonSerializer.Serialize(this,new JsonSerializerOptions(){ WriteIndented = true }); } catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; } }
}

/**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/main/*'/>*/
public static class CommandDetailsExtensions
{
    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="CreateActivity"]/*'/>*/
    public static Activity? CreateActivity(this CommandDetails details) { try { return Activity.CreateActivity(details); } catch { return null; } }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="GetAccessKey"]/*'/>*/
    public static AccessKey? GetAccessKey(this CommandDetails? details)
    {
        try { return details?.Arguments?.ContainsKey("AccessKey") is true ? details?.Arguments!["AccessKey"] as AccessKey : null; } catch { return null; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="GetArgument"]/*'/>*/
    public static Object? GetArgument(this CommandDetails? details , String? name)
    {
        try { return String.IsNullOrEmpty(name) is false && details?.Arguments?.ContainsKey(name) is true ? details?.Arguments![name!] : null; } catch { return null; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="GetArgumentType"]/*'/>*/
    public static TResult? GetArgument<TResult>(this CommandDetails? details , String? name)
    {
        try { return String.IsNullOrEmpty(name) is false && details?.Arguments?.ContainsKey(name) is true ? (TResult?)details?.Arguments![name!] : default; } catch { return default; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="GetArgumentWeb"]/*'/>*/
    public static String? GetArgument(this CommandDetailsWeb? details , String? name)
    {
        try { return String.IsNullOrEmpty(name) is false && details?.Arguments?.ContainsKey(name) is true ? details?.Arguments![name!] : null; } catch { return null; }
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetAccessKey"]/*'/>*/
    public static CommandDetails SetAccessKey(this CommandDetails details , AccessKey? key = null , Boolean? overwrite = false)
    {
        try
        {
            if(details is null || key is null) { return details!; }

            if(details.Arguments is null) { details.Arguments = new Dictionary<String,Object?>(); }

            if(details.Arguments.ContainsKey("AccessKey") is false || overwrite is true) { details.Arguments["AccessKey"] = key; }
        }
        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetAccessKeyWeb"]/*'/>*/
    public static CommandDetailsWeb SetAccessKey(this CommandDetailsWeb details , SecurityKeyWeb? key = null , Boolean? overwrite = false)
    {
        try
        {
            if(details is null || key is null) { return details!; }

            if(details.Arguments!.ContainsKey("AccessKey") is false || overwrite is true) { details.Arguments["AccessKey"] = key.ToString(); }
        }
        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetArgument"]/*'/>*/
    public static CommandDetails SetArgument(this CommandDetails details , String? name = null , Object? argument = null , Boolean? overwrite = false)
    {
        try
        {
            if(details is null || String.IsNullOrEmpty(name)) { return details!; }

            if(details.Arguments is null) { details.Arguments = new Dictionary<String,Object?>(); }

            if(details.Arguments.ContainsKey(name) is false || overwrite is true) { details.Arguments[name] = argument; }
        }
        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetArgumentWeb"]/*'/>*/
    public static CommandDetailsWeb SetArgument(this CommandDetailsWeb details , String? name = null , String? argument = null , Boolean? overwrite = false)
    {
        try
        {
            if(details is null || String.IsNullOrEmpty(name)) { return details!; }

            if(details.Arguments!.ContainsKey(name) is false || overwrite is true) { details!.Arguments[name] = argument; }
        }
        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetHandle"]/*'/>*/
    public static CommandDetails SetHandle(this CommandDetails details , String? handle = null)
    {
        try { if(details is null || handle is null) { return details!; } details.Handle = handle; }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetHandleWeb"]/*'/>*/
    public static CommandDetailsWeb SetHandle(this CommandDetailsWeb details , String? handle = null)
    {
        try { if(details is null || handle is null) { return details!; } details.Handle = handle; }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetID"]/*'/>*/
    public static CommandDetails SetID(this CommandDetails details , Guid? id = null)
    {
        try { if(details is null || id is null) { return details!; } details.ID = id; }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetIDWeb"]/*'/>*/
    public static CommandDetailsWeb SetID(this CommandDetailsWeb details , Guid? id = null)
    {
        try { if(details is null || id is null) { return details!; } details.ID = id; }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetLogger"]/*'/>*/
    public static CommandDetails SetLogger(this CommandDetails details , ILogger? logger = null)
    {
        try { if(details is null || logger is null) { return details!; } details.SetArgument("Logger",logger); }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetWorkflow"]/*'/>*/
    public static CommandDetails SetWorkflow(this CommandDetails details , CommandWorkflow? workflow = null)
    {
        try { if(details is null || workflow is null) { return details!; } details.Workflow = workflow; }

        catch {} return details;
    }

    /**<include file='CommandDetails.xml' path='CommandDetails/class[@name="CommandDetailsExtensions"]/method[@name="SetWorkflowWeb"]/*'/>*/
    public static CommandDetailsWeb SetWorkflow(this CommandDetailsWeb details , CommandWorkflow? workflow = null)
    {
        try { if(details is null || workflow is null) { return details!; } details.Workflow = workflow; }

        catch {} return details;
    }
}
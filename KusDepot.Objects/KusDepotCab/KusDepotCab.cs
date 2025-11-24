namespace KusDepot;

/**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/main/*'/>*/
[Alias("KusDepot.KusDepotCab")]
[DataContract(Name = "KusDepotCab" , Namespace = "KusDepot")]
[GenerateSerializer(GenerateFieldIds = GenerateFieldIds.PublicProperties)]
public record KusDepotCab : IParsable<KusDepotCab>
{
    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/property[@name="Data"]/*'/>*/
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public String? Data { get; init; }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Type { get; init; }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/property[@name="Cargo"]/*'/>*/
    [DataMember(Name = "Cargo" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public Dictionary<String,KusDepotCab>? Cargo { get; init; } = new();

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/property[@name="Manifest"]/*'/>*/
    [DataMember(Name = "Manifest" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public Dictionary<String,String>? Manifest { get; init; } = new();

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetObject"]/*'/>*/
    public TResult? GetObject<TResult>() where TResult : class
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(TResult)) ?? false)
            {
                return

                    ( GetWorkflowExceptionData() as TResult ) ??

                    ( GetAccessRequestWeb() as TResult ) ??

                    ( GetSecurityKeyWeb() as TResult ) ??

                    ( GetCommandDetails() as TResult ) ??

                    ( GetAccessRequest() as TResult ) ??

                    ( GetSecurityKey() as TResult ) ??

                    ( GetDataContent() as TResult ) ??

                    ( GetDescriptor() as TResult ) ??

                    ( GetToolOutput() as TResult ) ??

                    ( GetToolInput() as TResult ) ??

                    ( GetToolData() as TResult ) ??

                    ( GetDataItem() as TResult ) ??

                    ( GetKeySet() as TResult );
            }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetAccessRequest"]/*'/>*/
    public AccessRequest? GetAccessRequest()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(AccessRequest)) ?? false ) { return AccessRequest.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetAccessRequestWeb"]/*'/>*/
    public AccessRequestWeb? GetAccessRequestWeb()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(AccessRequestWeb)) ?? false ) { return AccessRequestWeb.Parse(this.Data); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetCommandDetails"]/*'/>*/
    public CommandDetails? GetCommandDetails()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(CommandDetails)) ?? false ) { return CommandDetails.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetDataItem"]/*'/>*/
    public DataItem? GetDataItem()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(TextItem)) ?? false )          { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(CodeItem)) ?? false )          { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(BinaryItem)) ?? false )        { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(DataSetItem)) ?? false )       { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(GenericItem)) ?? false )       { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(MSBuildItem)) ?? false )       { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(MultiMediaItem)) ?? false )    { goto ReturnData; }
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(GuidReferenceItem)) ?? false ) { goto ReturnData; }

            return null;

            ReturnData: return DataItem.Parse(this.Data);
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetDataContent"]/*'/>*/
    public DataContent? GetDataContent()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(DataContent)) ?? false ) { return DataContent.Parse(this.Data); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetDescriptor"]/*'/>*/
    public Descriptor? GetDescriptor()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(Descriptor)) ?? false ) { return Descriptor.Parse(this.Data); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetKeySet"]/*'/>*/
    public KeySet? GetKeySet()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(KeySet)) ?? false ) { return KeySet.Parse(this.Data); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetSecurityKey"]/*'/>*/
    public SecurityKey? GetSecurityKey()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(SecurityKey)) ?? false ) { return SecurityKey.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetSecurityKeyWeb"]/*'/>*/
    public SecurityKeyWeb? GetSecurityKeyWeb()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(SecurityKeyWeb)) ?? false ) { return SecurityKeyWeb.Parse(this.Data); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetToolData"]/*'/>*/
    public ToolData? GetToolData()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(ToolData)) ?? false ) { return ToolData.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetToolInput"]/*'/>*/
    public ToolInput? GetToolInput()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(ToolInput)) ?? false ) { return ToolInput.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetToolOutput"]/*'/>*/
    public ToolOutput? GetToolOutput()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(ToolOutput)) ?? false ) { return ToolOutput.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="GetWorkflowExceptionData"]/*'/>*/
    public WorkflowExceptionData? GetWorkflowExceptionData()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        try
        {
            if(System.Type.GetType(this.Type)?.IsAssignableTo(typeof(WorkflowExceptionData)) ?? false ) { return WorkflowExceptionData.Parse(this.Data,null); }

            return null;
        }
        catch { return null; }
    }

    ///<inheritdoc/>
    public override String ToString() { try { return JsonSerializer.Serialize(this,new JsonSerializerOptions(){ WriteIndented = true }); } catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; } }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse(String? input , IFormatProvider? format , out KusDepotCab result) 
    {
        result = null!; if(input is null) { return false; }

        try { var _ = Parse(input); if(_ is not null) { result = _; return true; } return false; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="IParsable{KusDepotCab}.Parse"]/*'/>*/
    static KusDepotCab IParsable<KusDepotCab>.Parse(String input , IFormatProvider? format) { return Parse(input)!; }

    /**<include file='KusDepotCab.xml' path='KusDepotCab/record[@name="KusDepotCab"]/method[@name="Parse"]/*'/>*/
    public static KusDepotCab? Parse(String input) { try { return String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<KusDepotCab>(input); } catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; } }
}
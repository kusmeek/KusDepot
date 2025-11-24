namespace KusDepot;

/**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("AccessRequest")]
[DataContract(Name = "AccessRequest" , Namespace = "KusDepot")]
public abstract class AccessRequest
{
    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Data"]/*'/>*/
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)] [NotNull] [Id(0)]
    public String? Data { get; protected set; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="ParseAny"]/*'/>*/
    public static AccessRequest? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null; } AccessRequest? r;

            try { r = ServiceRequest.Parse(input,format);    if(r is not null) { return r; } } catch { }

            try { r = StandardRequest.Parse(input,format);   if(r is not null) { return r; } } catch { }

            try { r = ManagementRequest.Parse(input,format); if(r is not null) { return r; } } catch { }

            return null;
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessRequest
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="ToAccessRequestWeb"]/*'/>*/
    public AccessRequestWeb ToAccessRequestWeb() { return new() { Data = this.Data }; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessRequest
    {
        result = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='AccessRequest.xml' path='AccessRequest/record[@name="AccessRequestWeb"]/main/*'/>*/
public record AccessRequestWeb
{
    /**<include file='AccessRequest.xml' path='AccessRequest/record[@name="AccessRequestWeb"]/property[@name="Data"]/*'/>*/
    public String? Data { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/record[@name="AccessRequestWeb"]/property[@name="Type"]/*'/>*/
    public String? Type { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/record[@name="AccessRequestWeb"]/method[@name="ToAccessRequest"]/*'/>*/
    public AccessRequest? ToAccessRequest()
    {
        if( String.IsNullOrEmpty(Data) || String.IsNullOrEmpty(Type) ) { return null; }

        switch(Type)
        {
            case nameof(StandardRequest):   return new StandardRequest(Data);

            case nameof(ManagementRequest): return new ManagementRequest(Data);

            case nameof(ServiceRequest):    return new ServiceRequest(null,Data);

            default: return null;
        }
    }

    ///<inheritdoc/>
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; } }

    /**<include file='AccessRequest.xml' path='AccessRequest/record[@name="AccessRequestWeb"]/method[@name="Parse"]/*'/>*/
    public static AccessRequestWeb? Parse(String input) { try { return String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<AccessRequestWeb>(input); } catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; } }
}
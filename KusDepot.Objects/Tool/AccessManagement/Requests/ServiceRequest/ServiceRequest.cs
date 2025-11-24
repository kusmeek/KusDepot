namespace KusDepot;

/**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/main/*'/>*/
[DataContract(Name = "ServiceRequest" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("ServiceRequest")]
public class ServiceRequest : AccessRequest , IParsable<ServiceRequest>
{
    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/property[@name="Tool"]/*'/>*/
    [NotNull]
    [IgnoreDataMember]
    [field: NonSerialized]
    public ITool? Tool { get; private set; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceRequest(ITool? tool , String data) { Data = data; Tool = tool; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="IParsable{ServiceRequest}.Parse"]/*'/>*/
    static ServiceRequest IParsable<ServiceRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="Parse"]/*'/>*/
    public static new ServiceRequest? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ServiceRequest)); ServiceRequest? _2 = _1.ReadObject(_0) as ServiceRequest; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="ToServiceRequestWeb"]/*'/>*/
    public ServiceRequestWeb ToServiceRequestWeb() { return new() { Data = this.Data }; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(ServiceRequest));

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch { return String.Empty; }
    }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ServiceRequest key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ServiceRequest)); ServiceRequest? _2 = _1.ReadObject(_0) as ServiceRequest; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ServiceRequest.xml' path='ServiceRequest/record[@name="ServiceRequestWeb"]/main/*'/>*/
public record ServiceRequestWeb : AccessRequestWeb
{
    /**<include file='ServiceRequest.xml' path='ServiceRequest/record[@name="ServiceRequestWeb"]/method[@name="ToServiceRequest"]/*'/>*/
    public ServiceRequest? ToServiceRequest() { return String.IsNullOrEmpty(Data) ? null : new(null,Data); }
}
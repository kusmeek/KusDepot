namespace KusDepot;

/**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/main/*'/>*/
[DataContract(Name = "ManagementRequest" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("ManagementRequest")]
public class ManagementRequest : AccessRequest , IParsable<ManagementRequest>
{
    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="Constructor"]/*'/>*/
    public ManagementRequest(String key) { Data = key; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ConstructorKey"]/*'/>*/
    public ManagementRequest(ManagementKey key) { Data = key.ToString(); }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="IParsable{ManagementRequest}.Parse"]/*'/>*/
    static ManagementRequest IParsable<ManagementRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="Parse"]/*'/>*/
    public static new ManagementRequest? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ManagementRequest)); ManagementRequest? _2 = _1.ReadObject(_0) as ManagementRequest; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="ToManagementRequestWeb"]/*'/>*/
    public ManagementRequestWeb ToManagementRequestWeb() { return new() { Data = this.Data }; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(ManagementRequest));

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch { return String.Empty; }
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ManagementRequest key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ManagementRequest)); ManagementRequest? _2 = _1.ReadObject(_0) as ManagementRequest; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ManagementRequest.xml' path='ManagementRequest/record[@name="ManagementRequestWeb"]/main/*'/>*/
public record ManagementRequestWeb : AccessRequestWeb
{
    /**<include file='ManagementRequest.xml' path='ManagementRequest/record[@name="ManagementRequestWeb"]/method[@name="ToManagementRequest"]/*'/>*/
    public ManagementRequest? ToManagementRequest() { return String.IsNullOrEmpty(Data) ? null : new(Data); }
}
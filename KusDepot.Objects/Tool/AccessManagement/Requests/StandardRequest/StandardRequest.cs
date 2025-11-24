namespace KusDepot;

/**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/main/*'/>*/
[DataContract(Name = "StandardRequest" , Namespace = "KusDepot")]
[GenerateSerializer] [Alias("StandardRequest")]
public class StandardRequest : AccessRequest , IParsable<StandardRequest>
{
    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/constructor[@name="Constructor"]/*'/>*/
    public StandardRequest(String data) { Data = data; }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="IParsable{StandardRequest}.Parse"]/*'/>*/
    static StandardRequest IParsable<StandardRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="Parse"]/*'/>*/
    public static new StandardRequest? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(StandardRequest)); StandardRequest? _2 = _1.ReadObject(_0) as StandardRequest; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="ToStandardRequestWeb"]/*'/>*/
    public StandardRequestWeb ToStandardRequestWeb() { return new() { Data = this.Data }; }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(StandardRequest));

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch { return String.Empty; }
    }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out StandardRequest key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(StandardRequest)); StandardRequest? _2 = _1.ReadObject(_0) as StandardRequest; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='StandardRequest.xml' path='StandardRequest/record[@name="StandardRequestWeb"]/main/*'/>*/
public record StandardRequestWeb : AccessRequestWeb
{
    /**<include file='StandardRequest.xml' path='StandardRequest/record[@name="StandardRequestWeb"]/method[@name="ToStandardRequest"]/*'/>*/
    public StandardRequest? ToStandardRequest() { return String.IsNullOrEmpty(Data) ? null : new(Data); }
}
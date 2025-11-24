namespace KusDepot;

/**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[DataContract(Name = "ToolData" , Namespace = "KusDepot")]
public class ToolData
{
    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/property[@name="Data"]/*'/>*/
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    public Object? Data { get; set; }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="Parse"]/*'/>*/
    public static ToolData? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ToolData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            ToolData? _2 = _1.ReadObject(_0) as ToolData; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(typeof(ToolData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ToolData data)
    {
        data = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ToolData),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            ToolData? _2 = _1.ReadObject(_0) as ToolData; if( _2 is not null ) { data = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}
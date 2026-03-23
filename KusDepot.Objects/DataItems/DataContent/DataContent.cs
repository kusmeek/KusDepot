namespace KusDepot;

/**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.DataContent")]
[DataContract(Name = "DataContent" , Namespace = "KusDepot")]

public sealed class DataContent : IParsable<DataContent>
{
    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="Binary"]/*'/>*/
    [DataMember(Name = "Binary" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Byte[]? Binary                            {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="Code"]/*'/>*/
    [DataMember(Name = "Code" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? Code                              {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="DataSet"]/*'/>*/
    [DataMember(Name = "DataSet" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public HashSet<DataItem>? DataSet                {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="Generic"]/*'/>*/
    [DataMember(Name = "Generic" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public List<Object>? Generic                     {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="GuidReference"]/*'/>*/
    [DataMember(Name = "GuidReference" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    public Dictionary<String,Object?>? GuidReference {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="MSBuild"]/*'/>*/
    [DataMember(Name = "MSBuild" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    public Dictionary<String,Object?>? MSBuild       {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="MultiMedia"]/*'/>*/
    [DataMember(Name = "MultiMedia" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    public Dictionary<String,Object?>?  MultiMedia   {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/property[@name="Text"]/*'/>*/
    [DataMember(Name = "Text" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    public String? Text                              {get;init;}

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    ///<inheritdoc/>
    public override String ToString()
    {
        try
        {
            return DataContractUtility.ToBase64String(this,SerializationData.ForType(typeof(DataContent)));
        }
        catch { return String.Empty; }
    }

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/method[@name="IParsable{DataContent}.Parse"]/*'/>*/
    static DataContent IParsable<DataContent>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/method[@name="Parse"]/*'/>*/
    public static DataContent? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            return DataContractUtility.ParseBase64<DataContent>(input,SerializationData.ForType(typeof(DataContent)));
        }
        catch { return null; }
    }

    /**<include file='DataContent.xml' path='DataContent/class[@name="DataContent"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out DataContent content)
    {
        content = null; if(input is null) { return false; }

        try
        {
            return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(DataContent)),out content);
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}
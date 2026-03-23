namespace KusDepot;

/**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/main/*'/>*/
[DataContract(Name = "ToolData" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[KnownType("GetKnownTypes")] [GenerateSerializer] [Alias("KusDepot.ToolData")]

public class ToolData
{
    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/property[@name="Data"]/*'/>*/
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)] [JsonIgnore] [Id(0)]
    public Object? Data { get; set; }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/property[@name="MyData"]/*'/>*/
    [JsonPropertyName("Data")] [JsonRequired] [IgnoreDataMember]
    public ToolValue? MyData
    {
        get { try { return ToolValueConverter.ToToolValue(this.Data); } catch { return null; } }

        set { try { this.Data = ToolValueConverter.FromToolValue(value); } catch {} }
    }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="Parse"]/*'/>*/
    public static ToolData? Parse(String input , IFormatProvider? format = null)
    {
        return DataContractUtility.ParseBase64<ToolData>(input,SerializationData.ForType(typeof(ToolData)));
    }

    ///<inheritdoc/>
    public override String ToString()
    {
        return DataContractUtility.ToBase64String(this,SerializationData.ForType(this.GetType()));
    }

    /**<include file='ToolData.xml' path='ToolData/class[@name="ToolData"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ToolData data)
    {
        return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(ToolData)),out data);
    }
}
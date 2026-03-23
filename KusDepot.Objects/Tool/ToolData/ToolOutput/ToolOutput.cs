namespace KusDepot;

/**<include file='ToolOutput.xml' path='ToolOutput/class[@name="ToolOutput"]/main/*'/>*/
[DataContract(Name = "ToolOutput" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[KnownType("GetKnownTypes")] [GenerateSerializer] [Alias("KusDepot.ToolOutput")]

public class ToolOutput : ToolData
{
    /**<include file='ToolOutput.xml' path='ToolOutput/class[@name="ToolOutput"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [NotNull] [JsonRequired] [Id(0)]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID { get; init; }

    /**<include file='ToolOutput.xml' path='ToolOutput/class[@name="ToolOutput"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    ///<inheritdoc/>
    public override String ToString() { return DataContractUtility.ToBase64String(this,SerializationData.ForType(this.GetType())); }

    /**<include file='ToolOutput.xml' path='ToolOutput/class[@name="ToolOutput"]/method[@name="Parse"]/*'/>*/
    public static new ToolOutput? Parse(String input , IFormatProvider? format = null)
    {
        return DataContractUtility.ParseBase64<ToolOutput>(input,SerializationData.ForType(typeof(ToolOutput)));
    }

    /**<include file='ToolOutput.xml' path='ToolOutput/class[@name="ToolOutput"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ToolOutput data)
    {
        return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(ToolOutput)),out data);
    }
}
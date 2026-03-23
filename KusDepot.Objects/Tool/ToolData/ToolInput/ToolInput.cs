namespace KusDepot;

/**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/main/*'/>*/
[DataContract(Name = "ToolInput" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[KnownType("GetKnownTypes")] [GenerateSerializer] [Alias("KusDepot.ToolInput")]

public class ToolInput : ToolData
{
    /**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    ///<inheritdoc/>
    public override String ToString() { return DataContractUtility.ToBase64String(this,SerializationData.ForType(this.GetType())); }

    /**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/method[@name="Parse"]/*'/>*/
    public static new ToolInput? Parse(String input , IFormatProvider? format = null)
    {
        return DataContractUtility.ParseBase64<ToolInput>(input,SerializationData.ForType(typeof(ToolInput)));
    }

    /**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ToolInput data)
    {
        return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(ToolInput)),out data);
    }
}
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
    public override String ToString() { return OrleansUtility.ToBase64String(this); }

    /**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/method[@name="Parse"]/*'/>*/
    public static new ToolInput? Parse(String input , IFormatProvider? format = null)
    {
        return OrleansUtility.ParseBase64<ToolInput>(input);
    }

    /**<include file='ToolInput.xml' path='ToolInput/class[@name="ToolInput"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ToolInput data)
    {
        return OrleansUtility.TryParseBase64(input,out data);
    }
}
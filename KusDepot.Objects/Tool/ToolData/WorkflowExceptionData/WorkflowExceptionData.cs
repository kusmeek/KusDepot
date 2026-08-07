namespace KusDepot;

/**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("KusDepot.WorkflowExceptionData")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "WorkflowExceptionData" , Namespace = "KusDepot")]

public sealed class WorkflowExceptionData : ToolData
{
    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Type"]/*'/>*/
    [JsonPropertyName("Type")] [JsonRequired] [NotNull] [Id(0)]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type { get; init; }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Message"]/*'/>*/
    [JsonPropertyName("Message")] [JsonRequired] [NotNull] [Id(1)]
    [DataMember(Name = "Message" , EmitDefaultValue = true , IsRequired = true)]
    public String? Message { get; init; }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="StackTrace"]/*'/>*/
    [JsonPropertyName("StackTrace")] [JsonRequired] [NotNull] [Id(2)]
    [DataMember(Name = "StackTrace" , EmitDefaultValue = true , IsRequired = true)]
    public String? StackTrace { get; init; }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="Create"]/*'/>*/
    public static WorkflowExceptionData Create(Exception source)
    {
        return new () {Type = source.GetType().FullName , Message = source.Message , StackTrace = source.StackTrace};
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="GetKnownTypes"]/*'/>*/
    public static new IEnumerable<Type> GetKnownTypes() => GetAllKnownTypes();

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="Parse"]/*'/>*/
    public static new WorkflowExceptionData? Parse(String input , IFormatProvider? format = null)
    {
        return OrleansUtility.ParseBase64<WorkflowExceptionData>(input);
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        return OrleansUtility.ToBase64String(this);
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out WorkflowExceptionData data)
    {
        return OrleansUtility.TryParseBase64(input,out data);
    }
}
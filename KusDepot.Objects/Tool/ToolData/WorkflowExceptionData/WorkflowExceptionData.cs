namespace KusDepot;

/**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "WorkflowExceptionData" , Namespace = "KusDepot")]

public sealed class WorkflowExceptionData : ToolData
{
    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Type"]/*'/>*/
    [JsonPropertyName("Type")] [JsonRequired] [NotNull]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type { get; init; }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="Message"]/*'/>*/
    [JsonPropertyName("Message")] [JsonRequired] [NotNull]
    [DataMember(Name = "Message" , EmitDefaultValue = true , IsRequired = true)]
    public String? Message { get; init; }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/property[@name="StackTrace"]/*'/>*/
    [JsonPropertyName("StackTrace")] [JsonRequired] [NotNull]
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
        return DataContractUtility.ParseBase64<WorkflowExceptionData>(input,SerializationData.ForType(typeof(WorkflowExceptionData)));
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        return DataContractUtility.ToBase64String(this,SerializationData.ForType(this.GetType()));
    }

    /**<include file='WorkflowExceptionData.xml' path='WorkflowExceptionData/class[@name="WorkflowExceptionData"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out WorkflowExceptionData data)
    {
        return DataContractUtility.TryParseBase64(input,SerializationData.ForType(typeof(WorkflowExceptionData)),out data);
    }
}
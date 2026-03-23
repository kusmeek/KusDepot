namespace KusDepot;

/**<include file='ToolValueArgumentParameter.xml' path='ToolValueArgumentParameter/record[@name="ToolValueArgumentParameter"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[GenerateSerializer] [Alias("KusDepot.ToolValueArgumentParameter")]
[DataContract(Name = "ToolValueArgumentParameter" , Namespace = "KusDepot")]

public sealed class ToolValueArgumentParameter
{
    /**<include file='ToolValueArgumentParameter.xml' path='ToolValueArgumentParameter/record[@name="ToolValueArgumentParameter"]/property[@name="Index"]/*'/>*/
    [Description(ToolValueParameterIndexDescription)]
    [JsonPropertyName("Index")] [JsonRequired] [Id(0)]
    [DataMember(Name = "Index" , EmitDefaultValue = true , IsRequired = true)]
    public Int32 Index { get; init; }

    /**<include file='ToolValueArgumentParameter.xml' path='ToolValueArgumentParameter/record[@name="ToolValueArgumentParameter"]/property[@name="Type"]/*'/>*/
    [Description(TypeNameDescription)]
    [JsonPropertyName("Type")] [JsonRequired] [Id(1)]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type { get; init; }

    /**<include file='ToolValueArgumentParameter.xml' path='ToolValueArgumentParameter/record[@name="ToolValueArgumentParameter"]/property[@name="Data"]/*'/>*/
    [Description(ToolValueParameterDataDescription)]
    [JsonPropertyName("Data")] [JsonRequired] [Id(2)]
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    public String? Data { get; init; }
}
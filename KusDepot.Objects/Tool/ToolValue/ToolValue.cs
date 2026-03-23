namespace KusDepot;

/**<include file='ToolValue.xml' path='ToolValue/record[@name="ToolValue"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.ToolValue")]
[DataContract(Name = "ToolValue" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public sealed class ToolValue
{
    /**<include file='ToolValue.xml' path='ToolValue/record[@name="ToolValue"]/property[@name="Mode"]/*'/>*/
    [Description(ToolValueModeDescription)]
    [JsonPropertyName("Mode")] [JsonRequired] [Id(0)]
    [DataMember(Name = "Mode" , EmitDefaultValue = true , IsRequired = true)]
    public ToolValueMode Mode { get; init; }

    /**<include file='ToolValue.xml' path='ToolValue/record[@name="ToolValue"]/property[@name="Type"]/*'/>*/
    [Description(TypeNameDescription)]
    [JsonPropertyName("Type")] [JsonRequired] [Id(1)]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type { get; init; }

    /**<include file='ToolValue.xml' path='ToolValue/record[@name="ToolValue"]/property[@name="Data"]/*'/>*/
    [Description(ToolValueDataDescription)]
    [JsonPropertyName("Data")] [JsonRequired] [Id(2)]
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    public String? Data { get; init; }

    /**<include file='ToolValue.xml' path='ToolValue/record[@name="ToolValue"]/property[@name="Arguments"]/*'/>*/
    [Description(ToolValueArgumentsDescription)]
    [JsonPropertyName("Arguments")] [JsonRequired] [Id(3)]
    [DataMember(Name = "Arguments" , EmitDefaultValue = true , IsRequired = true)]
    public ToolValueArgument[]? Arguments { get; init; }
}
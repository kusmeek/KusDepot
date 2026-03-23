namespace KusDepot;

/**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.ToolValueArgument")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ToolValueArgument" , Namespace = "KusDepot")]

public sealed class ToolValueArgument
{
    /**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/property[@name="Index"]/*'/>*/
    [Description(ToolValueArgumentIndexDescription)]
    [JsonPropertyName("Index")] [JsonRequired] [Id(0)]
    [DataMember(Name = "Index" , EmitDefaultValue = true , IsRequired = true)]
    public Int32 Index { get; init; }

    /**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/property[@name="Mode"]/*'/>*/
    [Description(ToolValueModeDescription)]
    [JsonPropertyName("Mode")] [JsonRequired] [Id(1)]
    [DataMember(Name = "Mode" , EmitDefaultValue = true , IsRequired = true)]
    public ToolValueMode Mode { get; init; }

    /**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/property[@name="Type"]/*'/>*/
    [Description(TypeNameDescription)]
    [JsonPropertyName("Type")] [JsonRequired] [Id(2)]
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    public String? Type { get; init; }

    /**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/property[@name="Data"]/*'/>*/
    [Description(ToolValueArgumentDataDescription)]
    [JsonPropertyName("Data")] [JsonRequired] [Id(3)]
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    public String? Data { get; init; }

    /**<include file='ToolValueArgument.xml' path='ToolValueArgument/record[@name="ToolValueArgument"]/property[@name="Parameters"]/*'/>*/
    [Description(ToolValueArgumentParametersDescription)]
    [JsonPropertyName("Parameters")] [JsonRequired] [Id(4)]
    [DataMember(Name = "Parameters" , EmitDefaultValue = true , IsRequired = true)]
    public ToolValueArgumentParameter[]? Parameters { get; init; }
}
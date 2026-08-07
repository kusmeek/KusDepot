namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferObjectInfo.xml' path='DataItemTransferObjectInfo/record[@name="DataItemTransferObjectInfo"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataItemTransferObjectInfo
{
    /**<include file='DataItemTransferObjectInfo.xml' path='DataItemTransferObjectInfo/record[@name="DataItemTransferObjectInfo"]/property[@name="ContentStreamed"]/*'/>*/
    [JsonPropertyName("ContentStreamed")] [JsonInclude]
    public Boolean? ContentStreamed { get; init; }

    /**<include file='DataItemTransferObjectInfo.xml' path='DataItemTransferObjectInfo/record[@name="DataItemTransferObjectInfo"]/property[@name="DataType"]/*'/>*/
    [JsonPropertyName("DataType")] [JsonInclude]
    public String? DataType { get; init; }

    /**<include file='DataItemTransferObjectInfo.xml' path='DataItemTransferObjectInfo/record[@name="DataItemTransferObjectInfo"]/property[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    public String? Name { get; init; }

    /**<include file='DataItemTransferObjectInfo.xml' path='DataItemTransferObjectInfo/record[@name="DataItemTransferObjectInfo"]/property[@name="ObjectType"]/*'/>*/
    [JsonPropertyName("ObjectType")] [JsonInclude]
    public String? ObjectType { get; init; }
}

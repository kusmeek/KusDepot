namespace KusDepot.Data.Services.DataTransfer;

/**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record PublishedTransferIntegrity
{
    /**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/property[@name="ObjectSHA512"]/*'/>*/
    [JsonPropertyName("ObjectSHA512")] [JsonInclude]
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PublishedTransferIntegrity.xml' path='PublishedTransferIntegrity/record[@name="PublishedTransferIntegrity"]/property[@name="StreamSHA512"]/*'/>*/
    [JsonPropertyName("StreamSHA512")] [JsonInclude]
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}

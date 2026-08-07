namespace KusDepot.Data.Clients;

/**<include file='PutTransferSegmentClientResponse.xml' path='PutTransferSegmentClientResponse/record[@name="PutTransferSegmentClientResponse"]/main/*'/>*/
public sealed record PutTransferSegmentClientResponse
{
    /**<include file='PutTransferSegmentClientResponse.xml' path='PutTransferSegmentClientResponse/record[@name="PutTransferSegmentClientResponse"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='PutTransferSegmentClientResponse.xml' path='PutTransferSegmentClientResponse/record[@name="PutTransferSegmentClientResponse"]/property[@name="Response"]/*'/>*/
    public PutTransferSegmentResponse? Response { get; init; }

    /**<include file='PutTransferSegmentClientResponse.xml' path='PutTransferSegmentClientResponse/record[@name="PutTransferSegmentClientResponse"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}

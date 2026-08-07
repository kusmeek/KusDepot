namespace KusDepot.Data.Clients;

/**<include file='PutStreamTransferSegmentClientResponse.xml' path='PutStreamTransferSegmentClientResponse/record[@name="PutStreamTransferSegmentClientResponse"]/main/*'/>*/
public sealed record PutStreamTransferSegmentClientResponse
{
    /**<include file='PutStreamTransferSegmentClientResponse.xml' path='PutStreamTransferSegmentClientResponse/record[@name="PutStreamTransferSegmentClientResponse"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='PutStreamTransferSegmentClientResponse.xml' path='PutStreamTransferSegmentClientResponse/record[@name="PutStreamTransferSegmentClientResponse"]/property[@name="Response"]/*'/>*/
    public PutStreamTransferSegmentResponse? Response { get; init; }

    /**<include file='PutStreamTransferSegmentClientResponse.xml' path='PutStreamTransferSegmentClientResponse/record[@name="PutStreamTransferSegmentClientResponse"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}

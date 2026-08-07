namespace KusDepot.Data.Clients;

/**<include file='StreamSegmentDownloadInfo.xml' path='StreamSegmentDownloadInfo/record[@name="StreamSegmentDownloadInfo"]/main/*'/>*/
public sealed record StreamSegmentDownloadInfo
{
    /**<include file='StreamSegmentDownloadInfo.xml' path='StreamSegmentDownloadInfo/record[@name="StreamSegmentDownloadInfo"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='StreamSegmentDownloadInfo.xml' path='StreamSegmentDownloadInfo/record[@name="StreamSegmentDownloadInfo"]/property[@name="Footer"]/*'/>*/
    public StreamTransferSegmentFooter Footer { get; init; } = new();

    /**<include file='StreamSegmentDownloadInfo.xml' path='StreamSegmentDownloadInfo/record[@name="StreamSegmentDownloadInfo"]/property[@name="Payload"]/*'/>*/
    public Byte[] Payload { get; init; } = Array.Empty<Byte>();

    /**<include file='StreamSegmentDownloadInfo.xml' path='StreamSegmentDownloadInfo/record[@name="StreamSegmentDownloadInfo"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}

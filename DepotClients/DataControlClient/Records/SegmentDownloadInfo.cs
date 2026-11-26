namespace KusDepot.Data.Clients;

/**<include file='SegmentDownloadInfo.xml' path='SegmentDownloadInfo/record[@name="SegmentDownloadInfo"]/main/*'/>*/
public sealed record SegmentDownloadInfo
{
    /**<include file='SegmentDownloadInfo.xml' path='SegmentDownloadInfo/record[@name="SegmentDownloadInfo"]/property[@name="Content"]/*'/>*/
    public String? Content { get; init; }

    /**<include file='SegmentDownloadInfo.xml' path='SegmentDownloadInfo/record[@name="SegmentDownloadInfo"]/property[@name="Footer"]/*'/>*/
    public TransferSegmentFooter Footer { get; init; } = new();

    /**<include file='SegmentDownloadInfo.xml' path='SegmentDownloadInfo/record[@name="SegmentDownloadInfo"]/property[@name="Payload"]/*'/>*/
    public Byte[] Payload { get; init; } = Array.Empty<Byte>();

    /**<include file='SegmentDownloadInfo.xml' path='SegmentDownloadInfo/record[@name="SegmentDownloadInfo"]/property[@name="StatusCode"]/*'/>*/
    public HttpStatusCode StatusCode { get; init; }
}

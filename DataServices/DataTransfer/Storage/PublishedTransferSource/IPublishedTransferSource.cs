namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IPublishedTransferSource.xml' path='IPublishedTransferSource/interface[@name="IPublishedTransferSource"]/main/*'/>*/
public interface IPublishedTransferSource
{
    /**<include file='IPublishedTransferSource.xml' path='IPublishedTransferSource/interface[@name="IPublishedTransferSource"]/method[@name="TryLoadSnapshot"]/*'/>*/
    Task<PublishedTransferSnapshot?> TryLoadSnapshot(Guid itemid , CancellationToken cancel = default);

    /**<include file='IPublishedTransferSource.xml' path='IPublishedTransferSource/interface[@name="IPublishedTransferSource"]/method[@name="OpenReadRange"]/*'/>*/
    Task<Stream?> OpenReadRange(PublishedTransferSnapshot snapshot , DataItemTransferRange range , CancellationToken cancel = default);
}

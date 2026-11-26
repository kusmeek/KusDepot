namespace DataPodServices.DataControl;

/**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/main/*'/>*/
internal interface ITenantTransferStorage
{
    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="AssembleStreamAsync"]/*'/>*/
    Task<PublishedArtifactReference?> AssembleStreamAsync(TransferBlockPlan plan , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="DeleteTransferAsync"]/*'/>*/
    Task<Boolean> DeleteTransferAsync(CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="OpenChunkAsync"]/*'/>*/
    Task<Stream?> OpenChunkAsync(Guid operationid , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="OpenObjectAsync"]/*'/>*/
    Task<Stream?> OpenObjectAsync(CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="OpenPublicationObjectAsync"]/*'/>*/
    Task<Stream?> OpenPublicationObjectAsync(Guid operationid , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="OpenRangeAsync"]/*'/>*/
    Task<Stream?> OpenRangeAsync(DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="PublishObjectAsync"]/*'/>*/
    Task<PublishedArtifactReference?> PublishObjectAsync(TenantTransferBlobReference source , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="ReplaceObjectAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> ReplaceObjectAsync(Stream payload , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SaveIntegrityAsync"]/*'/>*/
    Task<PublishedArtifactReference?> SaveIntegrityAsync(Stream payload , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SaveObjectAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> SaveObjectAsync(Stream payload , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SavePublicationObjectAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> SavePublicationObjectAsync(Guid operationid , Stream payload , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SaveRangeAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> SaveRangeAsync(DataItemTransferRange range , Stream payload , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SaveVerifiedChunkAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> SaveVerifiedChunkAsync(Guid operationid , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="SaveVerifiedRangeAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> SaveVerifiedRangeAsync(DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="TryLoadPublishedObjectAsync"]/*'/>*/
    Task<PublishedArtifactReference?> TryLoadPublishedObjectAsync(CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="TryLoadPublishedStreamAsync"]/*'/>*/
    Task<PublishedArtifactReference?> TryLoadPublishedStreamAsync(CancellationToken cancel = default);

    /**<include file='ITenantTransferStorage.xml' path='ITenantTransferStorage/interface[@name="ITenantTransferStorage"]/method[@name="TryLoadPublishedIntegrityAsync"]/*'/>*/
    Task<PublishedArtifactReference?> TryLoadPublishedIntegrityAsync(CancellationToken cancel = default);
}

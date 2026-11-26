namespace DataPodServices.DataControl;

/**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/main/*'/>*/
internal interface IDataControlTransferSessionStore
{
    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="CloseReadSessionAsync"]/*'/>*/
    Task<Boolean> CloseReadSessionAsync(Guid sessionid , Guid itemid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="CreateSegmentedBlockPlanAsync"]/*'/>*/
    Task<TransferBlockPlan?> CreateSegmentedBlockPlanAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="CreateSegmentedSessionAsync"]/*'/>*/
    Task<TransferBlobReference?> CreateSegmentedSessionAsync(DataItemTransferManifest manifest , Byte[] objectpayload , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="CreateStreamBlockPlanAsync"]/*'/>*/
    Task<TransferBlockPlan?> CreateStreamBlockPlanAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="CreateStreamSessionAsync"]/*'/>*/
    Task<TransferBlobReference?> CreateStreamSessionAsync(DataStreamTransferManifest manifest , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="DeleteSessionAsync"]/*'/>*/
    Task<Boolean> DeleteSessionAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="HasDependentSessionAsync"]/*'/>*/
    Task<Boolean> HasDependentSessionAsync(Guid sourcesessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadIntegritySegmentsAsync"]/*'/>*/
    Task<IReadOnlyList<TransferContentIntegritySegment>?> LoadIntegritySegmentsAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadObjectReferenceAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> LoadObjectReferenceAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadRecoveryStateForAdminAsync"]/*'/>*/
    Task<TransferRecoveryStateSnapshot> LoadRecoveryStateForAdminAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadSegmentedManifestAsync"]/*'/>*/
    Task<DataItemTransferManifest?> LoadSegmentedManifestAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadSegmentedWriteManifestAsync"]/*'/>*/
    Task<DataItemTransferManifest?> LoadSegmentedWriteManifestAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="IsSegmentedRangeCoveredAsync"]/*'/>*/
    Task<Boolean> IsSegmentedRangeCoveredAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadSessionsAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionState>> LoadSessionsAsync(TransferSessionQuery query , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadSessionsForAdminAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionState>> LoadSessionsForAdminAsync(TransferSessionQuery? query = null , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadStaleTransferSessionCandidatesAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionCleanupCandidate>> LoadStaleTransferSessionCandidatesAsync(DateTimeOffset cutoff , TransferSessionCleanupCandidate? after , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="LoadStreamManifestAsync"]/*'/>*/
    Task<DataStreamTransferManifest?> LoadStreamManifestAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="OpenObjectAsync"]/*'/>*/
    Task<Stream?> OpenObjectAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="OpenSegmentedRangeAsync"]/*'/>*/
    Task<Stream?> OpenSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="OpenStreamRangeAsync"]/*'/>*/
    Task<Stream?> OpenStreamRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="PurgeRecoveryStateAsync"]/*'/>*/
    Task<Boolean> PurgeRecoveryStateAsync(Guid sessionid , Guid itemid , Int64 expectedsessionstateversion , Guid? expectedoperationid , TransferPublicationPhase? expectedphase , DateTimeOffset? expectedjournalupdated , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="ReadObjectPayloadAsync"]/*'/>*/
    Task<Byte[]?> ReadObjectPayloadAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="RecomputeIntegrityAsync"]/*'/>*/
    Task<TransferContentIntegrity?> RecomputeIntegrityAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="ReplaceObjectAsync"]/*'/>*/
    Task<TransferBlobReference?> ReplaceObjectAsync(Guid sessionid , Stream payload , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="SaveObjectAsync"]/*'/>*/
    Task<TransferBlobReference?> SaveObjectAsync(Guid sessionid , Stream payload , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="SaveSegmentedManifestAsync"]/*'/>*/
    Task SaveSegmentedManifestAsync(DataItemTransferManifest manifest , TransferBlobReference? objectpayload = null , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="PromoteStagedReadManifestAsync"]/*'/>*/
    Task<Boolean> PromoteStagedReadManifestAsync(DataItemTransferManifest manifest , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="SaveStreamManifestAsync"]/*'/>*/
    Task SaveStreamManifestAsync(DataStreamTransferManifest manifest , TransferBlobReference? objectpayload = null , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="TouchTransferSessionAsync"]/*'/>*/
    Task<Boolean> TouchTransferSessionAsync(Guid sessionid , Guid itemid , DateTimeOffset activity , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="WriteSegmentedRangeAsync"]/*'/>*/
    Task<SegmentedTransferWriteResult> WriteSegmentedRangeAsync(Guid sessionid , DataItemTransferManifest manifest , DataItemTransferRange range , Stream payload , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="WriteVerifiedSegmentedRangeAsync"]/*'/>*/
    Task<SegmentedTransferWriteResult> WriteVerifiedSegmentedRangeAsync(Guid sessionid , DataItemTransferManifest manifest , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default);

    /**<include file='IDataControlTransferSessionStore.xml' path='IDataControlTransferSessionStore/interface[@name="IDataControlTransferSessionStore"]/method[@name="WriteVerifiedStreamChunkAsync"]/*'/>*/
    Task<StreamTransferAppendResult> WriteVerifiedStreamChunkAsync(Guid sessionid , Guid operationid , DataStreamTransferManifest manifest , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default);
}

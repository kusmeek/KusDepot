namespace DataPodServices.DataControl;

internal sealed partial class DataControlSegmentedTransferService
{
    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="TouchSessionActivity"]/*'/>*/
    private async Task TouchSessionActivity(Guid sessionid , Guid itemid , CancellationToken cancel)
    {
        if(await this.sessions.TouchTransferSessionAsync(sessionid,itemid,DateTimeOffset.UtcNow,cancel) is false)
        {
            throw new OperationFailedException(SessionNotFound,SegmentedTransferFailureCode.SessionNotFound);
        }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="LoadRequiredManifest"]/*'/>*/
    private async Task<DataItemTransferManifest> LoadRequiredManifest(Guid sessionid , Guid itemid , CancellationToken cancel)
    {
        return await LoadRequiredManifest(sessionid,itemid,false,cancel);
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="LoadRequiredManifestForWrite"]/*'/>*/
    private async Task<DataItemTransferManifest> LoadRequiredManifest(Guid sessionid , Guid itemid , Boolean forWrite , CancellationToken cancel)
    {
        if(sessionid == Guid.Empty || itemid == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        DataItemTransferManifest? manifest = forWrite
            ? await this.sessions.LoadSegmentedWriteManifestAsync(sessionid,cancel)
            : await this.sessions.LoadSegmentedManifestAsync(sessionid,cancel);

        if(manifest is null) { throw new OperationFailedException(SessionNotFound,SegmentedTransferFailureCode.SessionNotFound); }

        ValidateManifestIdentity(manifest,sessionid,itemid);

        CatalogItemState? current = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(itemid,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(manifest.ItemInstanceToken == Guid.Empty || current is not null && (current.IsPendingDelete || current.ItemInstanceToken != manifest.ItemInstanceToken))
        {
            throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState);
        }

        return forWrite ? manifest : manifest.NormalizeRanges();
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="CreateManifest"]/*'/>*/
    private DataItemTransferManifest CreateManifest(OpenUploadTransferRequest request , DataItemTransferSegmentSizePolicy policy , Guid itemInstanceToken)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            CatalogName = this.storageContext.CatalogName,
            ItemInstanceToken = itemInstanceToken,
            ObjectSHA512 = request.ObjectSHA512,
            StreamLength = request.StreamLength,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.Upload,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            SegmentSizePolicy = policy,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = request.ObjectInfo,
        };
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="CreateStagedReadManifest"]/*'/>*/
    private static DataItemTransferManifest CreateStagedReadManifest(OpenGetTransferRequest request , DataItemTransferManifest source)
    {
        return source with
        {
            SessionID = request.SessionID,
            SourceSessionID = source.SessionID,
            Mode = DataItemTransferMode.StagedRead,
            Status = DataItemTransferStatus.Open,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
        };
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="CreatePublishedReadManifest"]/*'/>*/
    private DataItemTransferManifest CreatePublishedReadManifest(OpenGetTransferRequest request , PublishedTransferSnapshot snapshot)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            CatalogName = this.storageContext.CatalogName,
            ItemInstanceToken = snapshot.ItemInstanceToken,
            ObjectSHA512 = snapshot.ObjectSHA512,
            Integrity = snapshot.Integrity,
            StreamLength = snapshot.StreamLength,
            Status = snapshot.StreamLength == 0 ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.PublishedRead,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            SegmentSizePolicy = this.options.DefaultSegmentSizePolicy,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = snapshot.ObjectInfo,
        };
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="LoadRequiredPublishedSnapshot"]/*'/>*/
    private async Task<PublishedTransferSnapshot> LoadRequiredPublishedSnapshot(Guid itemid , Guid itemInstanceToken , CancellationToken cancel)
    {
        PublishedTransferSnapshot? snapshot = await this.publishedSource.TryLoadSnapshot(itemid,itemInstanceToken,cancel);

        if(snapshot is null) { throw new OperationFailedException(PublishedSnapshotUnavailable,SegmentedTransferFailureCode.PublishedSnapshotUnavailable); }

        await ValidatePublishedSnapshot(snapshot,itemid,itemInstanceToken,cancel);

        return snapshot;
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="CreateRange"]/*'/>*/
    private static DataItemTransferRange CreateRange(Int64 offset , Int64 length)
    {
        return new() { Offset = offset , Length = length };
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateIdentity"]/*'/>*/
    private static void ValidateIdentity(DataItemTransferIdentity identity)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateManifestIdentity"]/*'/>*/
    private static void ValidateManifestIdentity(DataItemTransferManifest manifest , Guid sessionid , Guid itemid)
    {
        if(manifest.SessionID != sessionid || manifest.ItemID != itemid || manifest.ItemInstanceToken == Guid.Empty) { throw new OperationFailedException(IdentityMismatch,SegmentedTransferFailureCode.IdentityMismatch); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateMutationScope"]/*'/>*/
    private static void ValidateMutationScope(ITransferMutationScope scope , Guid sessionid , Guid itemid)
    {
        if(scope is null || scope.SessionID != sessionid || scope.ItemID != itemid) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateAbortRequest"]/*'/>*/
    private static void ValidateAbortRequest(AbortTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateRemoveRequest"]/*'/>*/
    private static void ValidateRemoveRequest(RemoveTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateOpenGetRequest"]/*'/>*/
    private static void ValidateOpenGetRequest(OpenGetTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.SourceSessionID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateReOpenGetRequest"]/*'/>*/
    private static void ValidateReOpenGetRequest(ReOpenGetTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateReOpenRequest"]/*'/>*/
    private static void ValidateReOpenRequest(ReOpenUploadTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateGetTransferSegmentRequest"]/*'/>*/
    private static void ValidateGetTransferSegmentRequest(GetTransferSegmentRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.Length <= 0 || !CreateRange(request.Offset,request.Length).Validate()) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidatePublishRequest"]/*'/>*/
    private static void ValidatePublishRequest(PublishSegmentedTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.ExpectedStateVersion < 0) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidatePutRequest"]/*'/>*/
    private static void ValidatePutRequest(PutTransferSegmentRequest request , Stream payload)
    {
        if(request is null || payload is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.Length <= 0 || request.SegmentSHA512 is null) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateSegmentLength"]/*'/>*/
    private void ValidateSegmentLength(DataItemTransferManifest manifest , Int64 length)
    {
        DataItemTransferSegmentSizePolicy policy = manifest.SegmentSizePolicy;

        if(policy.Validate() is false) { throw new OperationFailedException(InvalidSegmentPolicy,SegmentedTransferFailureCode.InvalidArgument); }

        if(this.options.EnforceMaximumSegmentBytes && length > policy.MaxSegmentBytes || this.options.EnforceMinimumSegmentBytes && length < policy.MinSegmentBytes) { throw new OperationFailedException(InvalidSegmentLength,SegmentedTransferFailureCode.InvalidSegmentLength); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateSessionWritable"]/*'/>*/
    private static void ValidateSessionWritable(DataItemTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted or DataItemTransferStatus.Publishing or DataItemTransferStatus.Published) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateAbortableSession"]/*'/>*/
    private static void ValidateAbortableSession(DataItemTransferManifest manifest)
    {
        if(manifest.Mode is not DataItemTransferMode.Upload || manifest.Status is not (DataItemTransferStatus.Open or DataItemTransferStatus.Complete)) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateSessionReadable"]/*'/>*/
    private static void ValidateSessionReadable(DataItemTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateManifestHashes"]/*'/>*/
    private static void ValidateManifestHashes(DataItemTransferManifest manifest , Byte[] objecthash , Int64 streamlength)
    {
        if(!manifest.ObjectSHA512.AsSpan().SequenceEqual(objecthash) || manifest.StreamLength != streamlength) { throw new OperationFailedException(InvalidArgument,SegmentedTransferFailureCode.InvalidArgument); }
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidatePublishedSnapshot"]/*'/>*/
    private static async Task ValidatePublishedSnapshot(PublishedTransferSnapshot snapshot , Guid itemid , Guid itemInstanceToken , CancellationToken cancel)
    {
        if(snapshot.ItemId != itemid || snapshot.ItemInstanceToken != itemInstanceToken || snapshot.ObjectPayload is null || snapshot.ObjectPayload.Length == 0 || snapshot.ObjectPayload.LongLength == 0 || !snapshot.ObjectSHA512.AsSpan().SequenceEqual(SHA512.HashData(snapshot.ObjectPayload))) { throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot); }

        await Task.CompletedTask;
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidatePublishedSnapshotIdentity"]/*'/>*/
    private static void ValidatePublishedSnapshotIdentity(PublishedTransferSnapshot snapshot , DataItemTransferManifest manifest)
    {
        Boolean sameIntegrity = manifest.Integrity is null || snapshot.Integrity is not null
            && String.Equals(snapshot.Integrity.Algorithm,manifest.Integrity.Algorithm,Ordinal)
            && snapshot.Integrity.Length == manifest.Integrity.Length
            && snapshot.Integrity.SegmentCount == manifest.Integrity.SegmentCount
            && snapshot.Integrity.Digest.AsSpan().SequenceEqual(manifest.Integrity.Digest);

        if(snapshot.ItemId != manifest.ItemID
            || snapshot.ItemInstanceToken != manifest.ItemInstanceToken
            || snapshot.StreamLength != manifest.StreamLength
            || !snapshot.ObjectSHA512.AsSpan().SequenceEqual(manifest.ObjectSHA512)
            || !sameIntegrity)
        {
            throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot);
        }
    }
}

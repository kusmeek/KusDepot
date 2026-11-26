namespace DataPodServices.DataControl;

internal sealed partial class DataControlStreamTransferService
{
    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="TouchSessionActivity"]/*'/>*/
    private async Task TouchSessionActivity(Guid sessionid , Guid itemid , CancellationToken cancel)
    {
        if(await this.sessions.TouchTransferSessionAsync(sessionid,itemid,DateTimeOffset.UtcNow,cancel) is false)
        {
            throw new OperationFailedException(SessionNotFound,StreamTransferFailureCode.SessionNotFound);
        }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="LoadRequiredManifest"]/*'/>*/
    private async Task<DataStreamTransferManifest> LoadRequiredManifest(Guid sessionID , Guid itemID , CancellationToken cancel)
    {
        if(sessionID == Guid.Empty || itemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        DataStreamTransferManifest? manifest = await this.sessions.LoadStreamManifestAsync(sessionID,cancel);

        if(manifest is null) { throw new OperationFailedException(SessionNotFound,StreamTransferFailureCode.SessionNotFound); }

        ValidateManifestIdentity(manifest,sessionID,itemID);

        CatalogItemState? current = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(itemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(manifest.ItemInstanceToken == Guid.Empty || current is not null && (current.IsPendingDelete || current.ItemInstanceToken != manifest.ItemInstanceToken))
        {
            throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState);
        }

        return manifest;
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="CreateOpenManifest"]/*'/>*/
    private DataStreamTransferManifest CreateOpenManifest(OpenStreamTransferRequest request , DataItemTransferSegmentSizePolicy policy , Guid itemInstanceToken)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            CatalogName = this.storageContext.CatalogName,
            ItemInstanceToken = itemInstanceToken,
            ObjectInfo = request.ObjectInfo,
            ObjectPayload = request.ObjectPayload,
            ObjectSHA512 = request.ObjectSHA512,
            SegmentSizePolicy = policy,
            AppendedLength = 0,
            StateVersion = 0,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.StreamUpload,
            Integrity = null,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="CreateFollowerManifest"]/*'/>*/
    private static DataStreamTransferManifest CreateFollowerManifest(OpenFollowStreamTransferRequest request , DataStreamTransferManifest source)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            CatalogName = source.CatalogName,
            ItemInstanceToken = source.ItemInstanceToken,
            SourceSessionID = source.SessionID,
            ObjectInfo = source.ObjectInfo,
            ObjectPayload = source.ObjectPayload,
            ObjectSHA512 = source.ObjectSHA512,
            SegmentSizePolicy = source.SegmentSizePolicy,
            AppendedLength = source.AppendedLength,
            StateVersion = 0,
            Status = source.Status,
            Mode = DataItemTransferMode.StreamFollow,
            Integrity = source.Integrity,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="SynchronizeFollowerManifest"]/*'/>*/
    private static DataStreamTransferManifest SynchronizeFollowerManifest(DataStreamTransferManifest follower , DataStreamTransferManifest source)
    {
        return follower with
        {
            AppendedLength = source.AppendedLength,
            FaultMessage = source.FaultMessage,
            ObjectInfo = source.ObjectInfo,
            ObjectPayload = source.ObjectPayload,
            ObjectSHA512 = source.ObjectSHA512,
            Integrity = source.Integrity,
            Status = source.Status,
            StateVersion = follower.StateVersion + 1,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ReadRequiredCompletionStreamHash"]/*'/>*/
    private async Task<Byte[]> ReadRequiredCompletionStreamHash(DataStreamTransferManifest manifest , CancellationToken cancel)
    {
        if(manifest.AppendedLength == 0) { return Array.Empty<Byte>(); }

        await using Stream? payload = await this.sessions.OpenStreamRangeAsync(manifest.SessionID,CreateRange(0,manifest.AppendedLength),cancel);

        if(payload is null) { throw new OperationFailedException(PublishFailed,StreamTransferFailureCode.PublishFailed); }

        return await SHA512.HashDataAsync(payload,cancel);
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ToServerSessionInfo"]/*'/>*/
    private static DataControlServerSessionInfo ToServerSessionInfo(DataStreamTransferManifest manifest)
    {
        return new()
        {
            ItemID = manifest.ItemID,
            SessionID = manifest.SessionID,
            StreamState = manifest.ToState(),
            SegmentedState = null,
            TransferFamily = DataControlTransferFamily.Stream,
        };
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="CreateRange"]/*'/>*/
    private static DataItemTransferRange CreateRange(Int64 offset , Int64 length)
    {
        return new() { Offset = offset , Length = length };
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateIdentity"]/*'/>*/
    private static void ValidateIdentity(DataItemTransferIdentity identity)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateManifestIdentity"]/*'/>*/
    private static void ValidateManifestIdentity(DataStreamTransferManifest manifest , Guid sessionID , Guid itemID)
    {
        if(manifest.SessionID != sessionID || manifest.ItemID != itemID || manifest.ItemInstanceToken == Guid.Empty) { throw new OperationFailedException(IdentityMismatch,StreamTransferFailureCode.IdentityMismatch); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateMutationScope"]/*'/>*/
    private static void ValidateMutationScope(ITransferMutationScope scope , Guid sessionID , Guid itemID)
    {
        if(scope is null || scope.SessionID != sessionID || scope.ItemID != itemID) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateAbortRequest"]/*'/>*/
    private static void ValidateAbortRequest(AbortTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateRemoveRequest"]/*'/>*/
    private static void ValidateRemoveRequest(RemoveTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateOpenFollowRequest"]/*'/>*/
    private static void ValidateOpenFollowRequest(OpenFollowStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.SourceSessionID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateReOpenFollowRequest"]/*'/>*/
    private static void ValidateReOpenFollowRequest(ReOpenFollowStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.SourceSessionID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateReOpenRequest"]/*'/>*/
    private static void ValidateReOpenRequest(ReOpenStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || !ValidOptionalHash(request.ObjectSHA512)) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateReadRequest"]/*'/>*/
    private static void ValidateReadRequest(GetStreamTransferSegmentRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.Length <= 0 || !CreateRange(request.Offset,request.Length).Validate()) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateWriteRequest"]/*'/>*/
    private static void ValidateWriteRequest(PutStreamTransferSegmentRequest request , Stream payload)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || payload is null || !payload.CanRead || request.Length <= 0 || !CreateRange(request.Offset,request.Length).Validate() || request.ExpectedStateVersion < 0 || request.SegmentSHA512 is null || request.SegmentSHA512.Length != 64) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateCompleteRequest"]/*'/>*/
    private static void ValidateCompleteRequest(PublishStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.ExpectedStateVersion < 0 || (request.FinalStreamLength.HasValue && request.FinalStreamLength.Value < 0)) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidOptionalHash"]/*'/>*/
    private static Boolean ValidOptionalHash(Byte[]? hash)
    {
        return hash is null || hash.Length is 0 or 64;
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateReadable"]/*'/>*/
    private static void ValidateReadable(DataStreamTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateAbortableSession"]/*'/>*/
    private static void ValidateAbortableSession(DataStreamTransferManifest manifest)
    {
        if(manifest.Mode is not DataItemTransferMode.StreamUpload || manifest.Status is not (DataItemTransferStatus.Open or DataItemTransferStatus.Complete)) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateWritableSource"]/*'/>*/
    private static void ValidateWritableSource(DataStreamTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Publishing or DataItemTransferStatus.Published or DataItemTransferStatus.Faulted || manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateSegmentLength"]/*'/>*/
    private void ValidateSegmentLength(DataStreamTransferManifest manifest , Int64 length)
    {
        DataItemTransferSegmentSizePolicy policy = manifest.SegmentSizePolicy;

        if(policy.Validate() is false) { throw new OperationFailedException(InvalidSegmentPolicy,StreamTransferFailureCode.InvalidArgument); }

        if(this.options.EnforceMaximumSegmentBytes && length > policy.MaxSegmentBytes || this.options.EnforceMinimumSegmentBytes && length < policy.MinSegmentBytes) { throw new OperationFailedException(InvalidSegmentLength,StreamTransferFailureCode.InvalidSegmentLength); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateHash"]/*'/>*/
    private static void ValidateHash(Byte[] actual , Byte[] expected , String message)
    {
        if(actual.AsSpan().SequenceEqual(expected) is false) { throw new OperationFailedException(message,StreamTransferFailureCode.StreamHashMismatch); }
    }

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/method[@name="ValidateOpenRequest"]/*'/>*/
    private static async Task ValidateOpenRequest(OpenStreamTransferRequest request , CancellationToken cancel)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.ObjectPayload is null || !ValidOptionalHash(request.ObjectSHA512) || request.RequestedSegmentSizePolicy is not null && !request.RequestedSegmentSizePolicy.Validate()) { throw new ArgumentException(InvalidArgument); }

        if(request.ObjectPayload.Length == 0 && request.ObjectSHA512.Length > 0 || request.ObjectPayload.Length > 0 && request.ObjectSHA512.Length != 64) { throw new ArgumentException(InvalidArgument); }

        if(request.ObjectPayload.Length > 0 && !SHA512.HashData(request.ObjectPayload).AsSpan().SequenceEqual(request.ObjectSHA512)) { throw new OperationFailedException(InvalidObjectHash,StreamTransferFailureCode.InvalidObjectHash); }

        await Task.CompletedTask;

        cancel.ThrowIfCancellationRequested();
    }
}

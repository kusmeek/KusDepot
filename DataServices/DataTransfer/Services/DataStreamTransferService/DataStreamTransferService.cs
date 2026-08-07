using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/main/*'/>*/
public sealed partial class DataStreamTransferService : IDataStreamTransferService
{
    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/field[@name="Coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator Coordinator;

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/field[@name="Registry"]/*'/>*/
    private readonly IDataItemTransferRegistry Registry;

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/field[@name="Storage"]/*'/>*/
    private readonly IStreamTransferStorage Storage;

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/field[@name="Options"]/*'/>*/
    private readonly DataItemTransferServiceOptions Options;

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/constructor[@name="DataStreamTransferService"]/*'/>*/
    public DataStreamTransferService(IStreamTransferStorage storage , IDataItemTransferRegistry registry , IDataItemTransferCoordinator coordinator , IOptions<DataItemTransferServiceOptions> options)
    {
        this.Storage = storage; this.Registry = registry; this.Coordinator = coordinator;

        this.Options = options?.Value ?? new DataItemTransferServiceOptions();
    }

    ///<inheritdoc/>
    public async Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateAbortRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(await this.Storage.DeleteSession(request.SessionID,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamAbortFailed,StreamTransferFailureCode.AbortFailed); }

                return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Aborted = true };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamAbortFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<CompleteStreamTransferResponse> BeginCompleteStreamTransfer(CompleteStreamTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateCompleteRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

                ValidateWritableSource(manifest);

                if(manifest.StateVersion != request.ExpectedStateVersion) { throw new OperationFailedException(StreamInvalidStateVersion,StreamTransferFailureCode.InvalidStateVersion); }

                if(request.FinalStreamLength.HasValue && request.FinalStreamLength.Value != manifest.AppendedLength) { throw new OperationFailedException(StreamInvalidArgument,StreamTransferFailureCode.InvalidArgument); }

                DataStreamTransferManifest committing = manifest.WithStatus(DataItemTransferStatus.Committing);

                if(await this.Storage.SaveManifest(committing,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamBeginCompleteFailed,StreamTransferFailureCode.CompleteFailed); }

                Byte[] streamHash = await ReadRequiredCompletionStreamHash(committing,cancel).ConfigureAwait(false);

                if(request.FinalStreamSHA512.Length == 64) { ValidateHash(streamHash,request.FinalStreamSHA512,StreamTransferStreamHashMismatch); }

                if(committing.StreamSHA512.Length == 64) { ValidateHash(streamHash,committing.StreamSHA512,StreamTransferStreamHashMismatch); }

                DataStreamTransferManifest completed = FinalizeSourceManifest(committing,request,streamHash);

                if(request.FinalObjectPayload.Length > 0)
                {
                    if(await this.Storage.SaveObjectPayload(completed.SessionID,request.FinalObjectPayload,cancel).ConfigureAwait(false) is false)
                    {
                        throw new OperationFailedException(StreamBeginCompleteFailed,StreamTransferFailureCode.CompleteFailed);
                    }
                }

                if(await this.Storage.SaveManifest(completed,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamBeginCompleteFailed,StreamTransferFailureCode.CompleteFailed); }

                return new() { State = completed.ToState() , Published = false };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamBeginCompleteFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<GetStreamTransferStateResponse> CancelCompleteStreamTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        try
        {
            if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(identity.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,identity.SessionID,identity.ItemID);

                if(manifest.Mode is not DataItemTransferMode.StreamUpload || manifest.Status is not DataItemTransferStatus.Committed)
                {
                    throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState);
                }

                DataStreamTransferManifest restored = manifest.WithStatus(DataItemTransferStatus.Open,null);

                if(await this.Storage.SaveManifest(restored,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamCancelCompleteFailed,StreamTransferFailureCode.CompleteFailed); }

                return new() { State = restored.ToState() };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamCancelCompleteFailed,identity?.SessionID,identity?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<CompleteStreamTransferResponse> FinishCompleteStreamTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        try
        {
            if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(identity.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,identity.SessionID,identity.ItemID);

                if(manifest.Mode is not DataItemTransferMode.StreamUpload || manifest.Status is not DataItemTransferStatus.Committed)
                {
                    throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState);
                }

                return new() { State = manifest.ToState() , Published = false };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamFinishCompleteFailed,identity?.SessionID,identity?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<StreamTransferReadResult> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateReadRequest(request);

            DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

            ValidateFollowerReadable(manifest);

            DataItemTransferRange range = CreateRange(request.Offset,request.Length);

            DataStreamTransferManifest readable = manifest;

            Guid sourceSessionId = manifest.SessionID;

            if(manifest.Mode is DataItemTransferMode.StreamFollow)
            {
                if(manifest.SourceSessionID.HasValue is false) { throw new OperationFailedException(StreamSourceSessionNotFound,StreamTransferFailureCode.SourceSessionNotFound); }

                DataStreamTransferManifest sourceManifest = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel).ConfigureAwait(false);

                ValidateSourceReadable(sourceManifest);

                sourceSessionId = sourceManifest.SessionID;

                readable = sourceManifest;
            }
            else if(manifest.Mode is not DataItemTransferMode.StreamUpload)
            {
                throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState);
            }

            if(readable.CanReadRange(range) is false) { throw new OperationFailedException(StreamRangeUnavailable,StreamTransferFailureCode.RangeUnavailable); }

            Stream? payload = await this.Storage.ReadRange(sourceSessionId,range,cancel).ConfigureAwait(false);

            if(payload is null) { throw new OperationFailedException(StreamTransferReadFailed,StreamTransferFailureCode.TransferReadFailed); }

            return new()
            {
                Footer = new()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                    RequestedOffset = request.Offset,
                    RequestedLength = request.Length,
                    ReturnedOffset = request.Offset,
                    ReturnedLength = request.Length,
                    AppendedLength = readable.AppendedLength,
                    StateVersion = readable.StateVersion,
                },
                Payload = new HashingReadStream(payload),
            };
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamTransferReadFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<GetStreamTransferStateResponse> GetStreamTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        ValidateStateRequest(identity);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default)
    {
        if(request?.TransferFamily.HasValue is true && request.TransferFamily.Value is not DataControlTransferFamily.Stream)
        {
            return Task.FromResult(Array.Empty<DataControlServerSessionInfo>());
        }

        System.Collections.Generic.List<DataControlServerSessionInfo> sessions = new();

        foreach(DataStreamTransferManifest manifest in EnumerateStoredManifests(request,cancel))
        {
            sessions.Add(ToServerSessionInfo(manifest));
        }

        return Task.FromResult(sessions.ToArray());
    }

    ///<inheritdoc/>
    public async Task<OpenFollowStreamTransferResponse> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateOpenFollowRequest(request);

            IAsyncDisposable itemLock = await this.Coordinator.AcquireItemLockAsync(request.ItemID,cancel).ConfigureAwait(false);

            await using (itemLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest sourceManifest = await LoadRequiredManifest(request.SourceSessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(sourceManifest,request.SourceSessionID,request.ItemID);

                ValidateSourceReadable(sourceManifest);

                DataStreamTransferManifest followerManifest = CreateFollowerManifest(request,sourceManifest);

                if(await this.Storage.CreateSession(followerManifest,cancel).ConfigureAwait(false) is null) { throw new OperationFailedException(StreamOpenFailed,StreamTransferFailureCode.OpenFailed); }

                return new()
                {
                    ObjectPayload = followerManifest.ObjectPayload,
                    State = followerManifest.ToState()
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<OpenStreamTransferResponse> OpenStreamTransfer(OpenStreamTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            await ValidateOpenRequest(request,cancel).ConfigureAwait(false);

            IAsyncDisposable itemLock = await this.Coordinator.AcquireItemLockAsync(request.ItemID,cancel).ConfigureAwait(false);

            await using (itemLock.ConfigureAwait(false))
            {
                DataItemTransferOpenConflict? existing = await this.Registry.FindOpenConflictByItemId(request.ItemID,cancel).ConfigureAwait(false);

                if(existing is not null) { throw new OperationFailedException(StreamConflictOpenSession,StreamTransferFailureCode.ConflictOpenSession); }

                DataStreamTransferManifest manifest = CreateOpenManifest(request);

                if(await this.Storage.CreateSession(manifest,cancel).ConfigureAwait(false) is null) { throw new OperationFailedException(StreamOpenFailed,StreamTransferFailureCode.OpenFailed); }

                return new() { State = manifest.ToState() };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<PutStreamTransferSegmentResponse> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , CancellationToken cancel = default)
    {
        try
        {
            ValidateWriteRequest(request,payload);

            DataStreamTransferManifest preflightManifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(preflightManifest,request.SessionID,request.ItemID);

            if(preflightManifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

            ValidateWritableSource(preflightManifest);

            ValidateSegmentLengthAgainstPolicy(preflightManifest,request.Length);

            SeekablePayloadBuffer stagedPayload = await SeekablePayloadBuffer.Create(payload,request.Length,this.Options.TemporaryUploadPath,cancel).ConfigureAwait(false);

            await using (stagedPayload.ConfigureAwait(false))
            {
                IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

                await using (sessionLock.ConfigureAwait(false))
                {
                    DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                    ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                    if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

                    ValidateWritableSource(manifest);

                    ValidateSegmentLengthAgainstPolicy(manifest,request.Length);

                    if(request.ExpectedStateVersion.HasValue && manifest.StateVersion != request.ExpectedStateVersion.Value) { throw new OperationFailedException(StreamInvalidStateVersion,StreamTransferFailureCode.InvalidStateVersion); }

                    if(request.Offset != manifest.AppendedLength) { throw new OperationFailedException(StreamAppendOffsetMismatch,StreamTransferFailureCode.AppendOffsetMismatch); }

                    if(request.StreamSHA512.Length == 64 && manifest.StreamSHA512.Length == 64) { ValidateHash(request.StreamSHA512,manifest.StreamSHA512,StreamTransferStreamHashMismatch); }

                    await ValidateSegmentHash(request.SegmentSHA512,stagedPayload.Stream,request.Length,cancel).ConfigureAwait(false);

                    DataItemTransferRange range = CreateRange(request.Offset,request.Length);

                    if(stagedPayload.Stream.CanSeek) { stagedPayload.Stream.Position = 0; }

                    StreamTransferAppendResult appended = await this.Storage.WriteTail(request.SessionID,range,stagedPayload.Stream,cancel).ConfigureAwait(false);

                    if(appended.Accepted is false)
                    {
                        return new()
                        {
                            SessionID = request.SessionID,
                            ItemID = request.ItemID,
                            Accepted = false,
                            AppendedLength = manifest.AppendedLength,
                            StateVersion = manifest.StateVersion,
                        };
                    }

                    DataStreamTransferManifest updated = manifest.WithAppendedLength(appended.AppendedLength);

                    if(ValidateOptionalHashBytes(request.StreamSHA512) && ValidateOptionalHashBytes(updated.StreamSHA512) is false)
                    {
                        updated = updated with { StreamSHA512 = request.StreamSHA512 };
                    }

                    if(await this.Storage.SaveManifest(updated,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamAppendFailed,StreamTransferFailureCode.AppendFailed); }

                    return new()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                        Accepted = true,
                        AppendedLength = updated.AppendedLength,
                        StateVersion = updated.StateVersion,
                    };
                }
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamAppendFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateRemoveRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(manifest.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Committed) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

                if(await this.Storage.DeleteSession(request.SessionID,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamRemoveFailed,StreamTransferFailureCode.RemoveFailed); }

                return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Removed = true };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamRemoveFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { return false; }

        DataStreamTransferManifest? manifest = await this.Storage.LoadManifest(identity.SessionID,cancel).ConfigureAwait(false);

        return manifest?.ItemID == identity.ItemID;
    }

    ///<inheritdoc/>
    public async Task<ReOpenFollowStreamTransferResponse> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateReOpenFollowerRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(manifest.Mode is not DataItemTransferMode.StreamFollow || manifest.SourceSessionID != request.SourceSessionID) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

                DataStreamTransferManifest sourceManifest = await LoadRequiredManifest(request.SourceSessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateSourceReadable(sourceManifest);

                DataStreamTransferManifest updated = SynchronizeFollowerManifest(manifest,sourceManifest);

                if(await this.Storage.SaveManifest(updated,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(StreamReOpenFailed,StreamTransferFailureCode.ReOpenFailed); }

                return new()
                {
                    ObjectPayload = updated.ObjectPayload,
                    State = updated.ToState()
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamReOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<ReOpenStreamTransferResponse> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateReOpenRequest(request);

            DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

            if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

            ValidateWritableSource(manifest);

            if(ValidateOptionalHashBytes(request.ObjectSHA512)) { ValidateHash(request.ObjectSHA512,manifest.ObjectSHA512,StreamInvalidObjectHash); }

            if(request.StreamSHA512.Length == 64) { ValidateHash(request.StreamSHA512,manifest.StreamSHA512,StreamTransferStreamHashMismatch); }

            return new() { State = manifest.ToState() };
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,StreamReOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }
}

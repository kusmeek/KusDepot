using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

public sealed partial class DataStreamTransferService
{
    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ClampPolicy"]/*'/>*/
    private static DataItemTransferSegmentSizePolicy ClampPolicy(DataItemTransferSegmentSizePolicy requested , DataItemTransferSegmentSizePolicy servicedefault)
    {
        if(requested?.Validate() is not true) { return servicedefault; }

        if(servicedefault.Validate() is false) { return requested; }

        Int64 min = Math.Max(requested.MinSegmentBytes,servicedefault.MinSegmentBytes);

        Int64 max = Math.Min(requested.MaxSegmentBytes,servicedefault.MaxSegmentBytes);

        if(max < min) { max = min; }

        Int64 preferred = requested.PreferredSegmentBytes;

        if(preferred < min) { preferred = min; }

        if(preferred > max) { preferred = max; }

        return new() { MinSegmentBytes = min , PreferredSegmentBytes = preferred , MaxSegmentBytes = max };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="CreateOpenManifest"]/*'/>*/
    private DataStreamTransferManifest CreateOpenManifest(OpenStreamTransferRequest request)
    {
        DataItemTransferSegmentSizePolicy effectivePolicy = ClampPolicy(request.RequestedSegmentSizePolicy ?? this.Options.DefaultSegmentSizePolicy,this.Options.DefaultSegmentSizePolicy);

        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            SourceSessionID = null,
            ObjectInfo = request.ObjectInfo,
            ObjectPayload = request.ObjectPayload,
            ObjectSHA512 = request.ObjectSHA512,
            SegmentSizePolicy = effectivePolicy,
            AppendedLength = 0,
            StateVersion = 0,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.StreamUpload,
            StreamSHA512 = Array.Empty<Byte>(),
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="CreateFollowerManifest"]/*'/>*/
    private static DataStreamTransferManifest CreateFollowerManifest(OpenFollowStreamTransferRequest request , DataStreamTransferManifest sourcemanifest)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            SourceSessionID = sourcemanifest.SessionID,
            ObjectInfo = sourcemanifest.ObjectInfo,
            ObjectPayload = sourcemanifest.ObjectPayload,
            ObjectSHA512 = sourcemanifest.ObjectSHA512,
            SegmentSizePolicy = sourcemanifest.SegmentSizePolicy,
            AppendedLength = sourcemanifest.AppendedLength,
            StateVersion = 0,
            Status = sourcemanifest.Status,
            Mode = DataItemTransferMode.StreamFollow,
            StreamSHA512 = sourcemanifest.StreamSHA512,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="CreateRange"]/*'/>*/
    private static DataItemTransferRange CreateRange(Int64 offset , Int64 length)
    {
        return new() { Offset = offset , Length = length };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="EnumerateStoredManifests"]/*'/>*/
    private IEnumerable<DataStreamTransferManifest> EnumerateStoredManifests(GetTransferSessionsRequest? request , CancellationToken cancel)
    {
        String root = Path.GetDirectoryName(this.Storage.GetSessionPaths(Guid.NewGuid()).SessionDirectoryPath) ?? String.Empty;

        if(String.IsNullOrWhiteSpace(root) || Directory.Exists(root) is false) { yield break; }

        foreach(String path in Directory.EnumerateFiles(root,DataStreamTransferManifest.ManifestFileName,SearchOption.AllDirectories))
        {
            cancel.ThrowIfCancellationRequested();

            DataStreamTransferManifest? manifest = DataStreamTransferManifest.Load(path);

            if(manifest is null || MatchesSessionFilter(manifest,request) is false) { continue; }

            yield return manifest;
        }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="FinalizeSourceManifest"]/*'/>*/
    private static DataStreamTransferManifest FinalizeSourceManifest(DataStreamTransferManifest manifest , CompleteStreamTransferRequest request , Byte[] streamhash)
    {
        DataItemTransferObjectInfo? objectInfo = request.FinalObjectInfo ?? manifest.ObjectInfo;

        Byte[] objectPayload = request.FinalObjectPayload.Length > 0 ? request.FinalObjectPayload : manifest.ObjectPayload;

        Byte[] objectHash = objectPayload.Length == 0 ? Array.Empty<Byte>() : (request.FinalObjectSHA512.Length > 0 ? request.FinalObjectSHA512 : manifest.ObjectSHA512);

        return manifest.WithStatus(DataItemTransferStatus.Committed) with
        {
            ObjectInfo = objectInfo,
            ObjectPayload = objectPayload,
            ObjectSHA512 = objectHash,
            StreamSHA512 = streamhash,
        };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="MatchesSessionFilter"]/*'/>*/
    private static Boolean MatchesSessionFilter(DataStreamTransferManifest manifest , GetTransferSessionsRequest? request)
    {
        if(request?.ItemID.HasValue is true && manifest.ItemID != request.ItemID.Value) { return false; }

        if(request?.SessionID.HasValue is true && manifest.SessionID != request.SessionID.Value) { return false; }

        if(request?.Status.HasValue is true && manifest.Status != request.Status.Value) { return false; }

        return request?.TransferFamily.HasValue is not true || request.TransferFamily.Value is DataControlTransferFamily.Stream;
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="LoadRequiredManifest"]/*'/>*/
    private async Task<DataStreamTransferManifest> LoadRequiredManifest(Guid sessionid , Guid itemid , CancellationToken cancel)
    {
        if(sessionid == Guid.Empty || itemid == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

        if(File.Exists(this.Storage.GetSessionPaths(sessionid).ManifestPath) is false)
        {
            throw new OperationFailedException($"StreamSessionNotFound SessionID: {sessionid} ItemID: {itemid}",StreamTransferFailureCode.SessionNotFound);
        }

        DataStreamTransferManifest? manifest = await this.Storage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        if(manifest is null) { throw new OperationFailedException($"StreamLoadSessionFailed SessionID: {sessionid} ItemID: {itemid}",StreamTransferFailureCode.LoadSessionFailed); }

        return manifest;
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ToServerSessionInfo"]/*'/>*/
    private static DataControlServerSessionInfo ToServerSessionInfo(DataStreamTransferManifest manifest)
    {
        DataStreamTransferState streamState = manifest.ToState();

        return new()
        {
            ItemID = manifest.ItemID,
            SegmentedState = null,
            SessionID = manifest.SessionID,
            StreamState = streamState,
            TransferFamily = DataControlTransferFamily.Stream,
        };
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="MapFailureCode"]/*'/>*/
    private static StreamTransferFailureCode MapFailureCode(String message)
    {
        if(String.Equals(message,StreamAppendOffsetMismatch,StringComparison.Ordinal))       { return StreamTransferFailureCode.AppendOffsetMismatch; }
        if(String.Equals(message,StreamCompleteFailed,StringComparison.Ordinal))             { return StreamTransferFailureCode.CompleteFailed; }
        if(String.Equals(message,StreamIdentityMismatch,StringComparison.Ordinal))           { return StreamTransferFailureCode.IdentityMismatch; }
        if(String.Equals(message,StreamInvalidArgument,StringComparison.Ordinal))            { return StreamTransferFailureCode.InvalidArgument; }
        if(String.Equals(message,StreamInvalidObjectHash,StringComparison.Ordinal))          { return StreamTransferFailureCode.InvalidObjectHash; }
        if(String.Equals(message,StreamInvalidSegmentHash,StringComparison.Ordinal))         { return StreamTransferFailureCode.InvalidSegmentHash; }
        if(String.Equals(message,StreamInvalidSegmentLength,StringComparison.Ordinal))       { return StreamTransferFailureCode.InvalidSegmentLength; }
        if(String.Equals(message,StreamInvalidSessionState,StringComparison.Ordinal))        { return StreamTransferFailureCode.InvalidSessionState; }
        if(String.Equals(message,StreamInvalidStateVersion,StringComparison.Ordinal))        { return StreamTransferFailureCode.InvalidStateVersion; }
        if(String.Equals(message,StreamLoadSessionFailed,StringComparison.Ordinal))          { return StreamTransferFailureCode.LoadSessionFailed; }
        if(String.Equals(message,StreamOpenFailed,StringComparison.Ordinal))                 { return StreamTransferFailureCode.OpenFailed; }
        if(String.Equals(message,StreamRangeUnavailable,StringComparison.Ordinal))           { return StreamTransferFailureCode.RangeUnavailable; }
        if(String.Equals(message,StreamReOpenFailed,StringComparison.Ordinal))               { return StreamTransferFailureCode.ReOpenFailed; }
        if(String.Equals(message,StreamRemoveFailed,StringComparison.Ordinal))               { return StreamTransferFailureCode.RemoveFailed; }
        if(String.Equals(message,StreamSessionNotFound,StringComparison.Ordinal))            { return StreamTransferFailureCode.SessionNotFound; }
        if(String.Equals(message,StreamSourceSessionNotFound,StringComparison.Ordinal))      { return StreamTransferFailureCode.SourceSessionNotFound; }
        if(String.Equals(message,StreamTransferReadFailed,StringComparison.Ordinal))         { return StreamTransferFailureCode.TransferReadFailed; }
        if(String.Equals(message,StreamTransferStreamHashMismatch,StringComparison.Ordinal)) { return StreamTransferFailureCode.StreamHashMismatch; }

        return StreamTransferFailureCode.InvalidArgument;
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ReadRequiredCompletionStreamHash"]/*'/>*/
    private async Task<Byte[]> ReadRequiredCompletionStreamHash(DataStreamTransferManifest manifest , CancellationToken cancel)
    {
        if(manifest.AppendedLength == 0) { return SHA512.HashData(Array.Empty<Byte>()); }

        using Stream? payload = await this.Storage.ReadRange(manifest.SessionID,CreateRange(0,manifest.AppendedLength),cancel).ConfigureAwait(false);

        if(payload is null) { throw new OperationFailedException(StreamCompleteFailed,StreamTransferFailureCode.CompleteFailed); }

        return await SHA512.HashDataAsync(payload,cancel).ConfigureAwait(false);
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="SynchronizeFollowerManifest"]/*'/>*/
    private static DataStreamTransferManifest SynchronizeFollowerManifest(DataStreamTransferManifest followermanifest , DataStreamTransferManifest sourcemanifest)
    {
        DataStreamTransferManifest updated = followermanifest with
        {
            AppendedLength = sourcemanifest.AppendedLength,
            FaultMessage = sourcemanifest.FaultMessage,
            ObjectInfo = sourcemanifest.ObjectInfo,
            ObjectPayload = sourcemanifest.ObjectPayload,
            ObjectSHA512 = sourcemanifest.ObjectSHA512,
            StreamSHA512 = sourcemanifest.StreamSHA512,
            Status = sourcemanifest.Status,
            Updated = DateTimeOffset.UtcNow,
            StateVersion = followermanifest.StateVersion + 1,
        };

        return updated;
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateAbortRequest"]/*'/>*/
    private static void ValidateAbortRequest(AbortTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateCompleteRequest"]/*'/>*/
    private static void ValidateCompleteRequest(CompleteStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.ExpectedStateVersion < 0) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.FinalStreamLength.HasValue && request.FinalStreamLength.Value < 0) { throw new ArgumentException(StreamInvalidArgument); }

        if(ValidateOptionalHashBytes(request.FinalObjectSHA512) is false || ValidateOptionalHashBytes(request.FinalStreamSHA512) is false) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.FinalObjectPayload.Length > 0 && request.FinalObjectSHA512.Length != 64) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.FinalObjectPayload.Length == 0 && request.FinalObjectSHA512.Length > 0) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateFollowerReadable"]/*'/>*/
    private static void ValidateFollowerReadable(DataStreamTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateHashBytes"]/*'/>*/
    private static void ValidateHash(Byte[] actual , Byte[] expected , String message)
    {
        if(actual.AsSpan().SequenceEqual(expected) is false) { throw new OperationFailedException(message,MapFailureCode(message)); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateHashPayloadAsync"]/*'/>*/
    private static async Task ValidateHash(Byte[] expectedhash , Stream payload , Int64 expectedlength , String message , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(payload); ArgumentOutOfRangeException.ThrowIfNegative(expectedlength);

        if(payload.CanSeek) { payload.Position = 0; }

        using Stream bounded = payload.CanSeek
            ? new BoundedReadStream(payload,payload.Position,expectedlength,leaveopen:true)
            : new NonSeekableBoundedReadStream(payload,expectedlength,leaveopen:true);

        Byte[] actual = await SHA512.HashDataAsync(bounded,cancel).ConfigureAwait(false);

        if(payload.CanSeek) { payload.Position = 0; }

        if(actual.AsSpan().SequenceEqual(expectedhash) is false) { throw new OperationFailedException(message,MapFailureCode(message)); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateManifestIdentity"]/*'/>*/
    private static void ValidateManifestIdentity(DataStreamTransferManifest manifest , Guid sessionid , Guid itemid)
    {
        if(manifest.SessionID != sessionid || manifest.ItemID != itemid) { throw new OperationFailedException(StreamIdentityMismatch,StreamTransferFailureCode.IdentityMismatch); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateOpenFollowRequest"]/*'/>*/
    private static void ValidateOpenFollowRequest(OpenFollowStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.SourceSessionID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateOpenRequest"]/*'/>*/
    private static async Task ValidateOpenRequest(OpenStreamTransferRequest request , CancellationToken cancel)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.RequestedSegmentSizePolicy is not null && request.RequestedSegmentSizePolicy.Validate() is false) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.ObjectPayload is null || ValidateOptionalHashBytes(request.ObjectSHA512) is false) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.ObjectPayload.Length > 0)
        {
            if(request.ObjectSHA512.Length != 64) { throw new ArgumentException(StreamInvalidArgument); }

            Byte[] hash = request.ObjectPayload.Length >= LargeObjectPayloadSize
                ? await ComputeSHA512Async(request.ObjectPayload,MiB,cancel).ConfigureAwait(false)
                : SHA512.HashData(request.ObjectPayload);

            if(hash.AsSpan().SequenceEqual(request.ObjectSHA512) is false) { throw new OperationFailedException(StreamInvalidObjectHash,StreamTransferFailureCode.InvalidObjectHash); }
        }
        else if(request.ObjectSHA512.Length > 0)
        {
            throw new ArgumentException(StreamInvalidArgument);
        }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateOptionalHashBytes"]/*'/>*/
    private static Boolean ValidateOptionalHashBytes(Byte[]? hash)
    {
        return hash is null || hash.Length == 0 || hash.Length == 64;
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateReadRequest"]/*'/>*/
    private static void ValidateReadRequest(GetStreamTransferSegmentRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

        if(CreateRange(request.Offset,request.Length).Validate() is false || request.Length <= 0) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateReOpenFollowerRequest"]/*'/>*/
    private static void ValidateReOpenFollowerRequest(ReOpenFollowStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.SourceSessionID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateReOpenRequest"]/*'/>*/
    private static void ValidateReOpenRequest(ReOpenStreamTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

        if(ValidateOptionalHashBytes(request.ObjectSHA512) is false || ValidateOptionalHashBytes(request.StreamSHA512) is false) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateRemoveRequest"]/*'/>*/
    private static void ValidateRemoveRequest(RemoveTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateSegmentHash"]/*'/>*/
    private static Task ValidateSegmentHash(Byte[] expectedhash , Stream payload , Int64 expectedlength , CancellationToken cancel)
    {
        if(expectedhash is null || expectedhash.Length != 64) { throw new ArgumentException(StreamInvalidArgument); }

        return ValidateHash(expectedhash,payload,expectedlength,StreamInvalidSegmentHash,cancel);
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateSegmentLengthAgainstPolicy"]/*'/>*/
    private void ValidateSegmentLengthAgainstPolicy(DataStreamTransferManifest manifest , Int64 length)
    {
        DataItemTransferSegmentSizePolicy policy = ClampPolicy(manifest.SegmentSizePolicy,this.Options.DefaultSegmentSizePolicy);

        if(policy.Validate() is false) { return; }

        if(this.Options.EnforceMaximumSegmentBytes && length > policy.MaxSegmentBytes) { throw new OperationFailedException(StreamInvalidSegmentLength,StreamTransferFailureCode.InvalidSegmentLength); }

        if(this.Options.EnforceMinimumSegmentBytes && length < policy.MinSegmentBytes) { throw new OperationFailedException(StreamInvalidSegmentLength,StreamTransferFailureCode.InvalidSegmentLength); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateSourceReadable"]/*'/>*/
    private static void ValidateSourceReadable(DataStreamTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateStateRequest"]/*'/>*/
    private static void ValidateStateRequest(DataItemTransferIdentity identity)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateWritableSource"]/*'/>*/
    private static void ValidateWritableSource(DataStreamTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Committing or DataItemTransferStatus.Committed or DataItemTransferStatus.Faulted)
        {
            throw new OperationFailedException(StreamInvalidSessionState,StreamTransferFailureCode.InvalidSessionState);
        }
    }

    /**<include file='DataStreamTransferService.xml' path='DataStreamTransferService/class[@name="DataStreamTransferService"]/method[@name="ValidateWriteRequest"]/*'/>*/
    private static void ValidateWriteRequest(PutStreamTransferSegmentRequest request , Stream payload)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(StreamInvalidArgument); }

        if(payload is null || payload.CanRead is false) { throw new ArgumentException(StreamInvalidArgument); }

        if(CreateRange(request.Offset,request.Length).Validate() is false || request.Length <= 0) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.ExpectedStateVersion.HasValue && request.ExpectedStateVersion.Value < 0) { throw new ArgumentException(StreamInvalidArgument); }

        if(ValidateOptionalHashBytes(request.StreamSHA512) is false) { throw new ArgumentException(StreamInvalidArgument); }

        if(request.SegmentSHA512 is null || request.SegmentSHA512.Length != 64) { throw new ArgumentException(StreamInvalidArgument); }
    }
}

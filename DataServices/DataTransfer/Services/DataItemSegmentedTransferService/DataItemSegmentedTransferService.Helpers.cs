using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

public sealed partial class DataItemSegmentedTransferService
{
    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ClampPolicy"]/*'/>*/
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

        return new()
        {
            MinSegmentBytes = min,
            PreferredSegmentBytes = preferred,
            MaxSegmentBytes = max,
        };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="CreateCommittedReadManifest"]/*'/>*/
    private DataItemTransferManifest CreateCommittedReadManifest(OpenGetTransferRequest request , PublishedTransferSnapshot snapshot)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            SourceSessionID = null,
            ObjectSHA512 = snapshot.ObjectSHA512,
            StreamSHA512 = snapshot.StreamSHA512,
            StreamLength = snapshot.StreamLength,
            Status = snapshot.StreamLength == 0 ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.ReadCommitted,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            SegmentSizePolicy = this.Options.DefaultSegmentSizePolicy,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = snapshot.ObjectInfo,
        };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="CreateManifest"]/*'/>*/
    private DataItemTransferManifest CreateManifest(OpenUploadTransferRequest request)
    {
        DataItemTransferSegmentSizePolicy effectivePolicy = ClampPolicy(request.RequestedSegmentSizePolicy ?? this.Options.DefaultSegmentSizePolicy,this.Options.DefaultSegmentSizePolicy);

        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            ObjectSHA512 = request.ObjectSHA512,
            StreamSHA512 = request.StreamSHA512,
            StreamLength = request.StreamLength,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.Upload,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            SegmentSizePolicy = effectivePolicy,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = request.ObjectInfo,
        };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="CreateRange"]/*'/>*/
    private static DataItemTransferRange CreateRange(Int64 offset , Int64 length)
    {
        return new() { Offset = offset , Length = length };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="EnumerateStoredManifests"]/*'/>*/
    private IEnumerable<DataItemTransferManifest> EnumerateStoredManifests(GetTransferSessionsRequest? request , CancellationToken cancel)
    {
        String root = Path.GetDirectoryName(this.Storage.GetSessionPaths(Guid.NewGuid()).SessionDirectoryPath) ?? String.Empty;

        if(String.IsNullOrWhiteSpace(root) || Directory.Exists(root) is false) { yield break; }

        foreach(String path in Directory.EnumerateFiles(root,DataItemTransferManifest.ManifestFileName,SearchOption.AllDirectories))
        {
            cancel.ThrowIfCancellationRequested();

            DataItemTransferManifest? manifest = DataItemTransferManifest.Load(path)?.NormalizeRanges();

            if(manifest is null || MatchesSessionFilter(manifest,request) is false) { continue; }

            yield return manifest;
        }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="CreateStagedReadManifest"]/*'/>*/
    private static DataItemTransferManifest CreateStagedReadManifest(OpenGetTransferRequest request , DataItemTransferManifest sourcemanifest)
    {
        return new()
        {
            SessionID = request.SessionID,
            ItemID = request.ItemID,
            SourceSessionID = sourcemanifest.SessionID,
            ObjectSHA512 = sourcemanifest.ObjectSHA512,
            StreamSHA512 = sourcemanifest.StreamSHA512,
            StreamLength = sourcemanifest.StreamLength,
            Status = DataItemTransferStatus.Open,
            Mode = DataItemTransferMode.ReadStaged,
            StateVersion = 0,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            SegmentSizePolicy = sourcemanifest.SegmentSizePolicy,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
            ObjectInfo = sourcemanifest.ObjectInfo,
        };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="LoadRequiredManifest"]/*'/>*/
    private async Task<DataItemTransferManifest> LoadRequiredManifest(Guid sessionid , Guid itemid , CancellationToken cancel)
    {
        if(sessionid == Guid.Empty || itemid == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(File.Exists(this.Storage.GetSessionPaths(sessionid).ManifestPath) is false)
        {
            throw new OperationFailedException($"SessionNotFound SessionID: {sessionid} ItemID: {itemid}",SegmentedTransferFailureCode.SessionNotFound);
        }

        DataItemTransferManifest? manifest = await this.Storage.LoadManifest(sessionid,cancel).ConfigureAwait(false);

        if(manifest is null) { throw new OperationFailedException($"LoadSessionFailed SessionID: {sessionid} ItemID: {itemid}",SegmentedTransferFailureCode.LoadSessionFailed); }

        return manifest.NormalizeRanges();
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="MatchesSessionFilter"]/*'/>*/
    private static Boolean MatchesSessionFilter(DataItemTransferManifest manifest , GetTransferSessionsRequest? request)
    {
        if(request?.ItemID.HasValue is true && manifest.ItemID != request.ItemID.Value) { return false; }

        if(request?.SessionID.HasValue is true && manifest.SessionID != request.SessionID.Value) { return false; }

        if(request?.Status.HasValue is true && manifest.Status != request.Status.Value) { return false; }

        return request?.TransferFamily.HasValue is not true || request.TransferFamily.Value is DataControlTransferFamily.Segmented;
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ToServerSessionInfo"]/*'/>*/
    private static DataControlServerSessionInfo ToServerSessionInfo(DataItemTransferManifest manifest)
    {
        DataItemTransferState state = manifest.ToState();

        return new()
        {
            ItemID = manifest.ItemID,
            SegmentedState = state,
            SessionID = manifest.SessionID,
            StreamState = null,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="LoadRequiredPublishedSnapshot"]/*'/>*/
    private async Task<PublishedTransferSnapshot> LoadRequiredPublishedSnapshot(Guid itemid , CancellationToken cancel)
    {
        PublishedTransferSnapshot? snapshot = await this.PublishedSource.TryLoadSnapshot(itemid,cancel).ConfigureAwait(false);

        if(snapshot is null) { throw new OperationFailedException(PublishedSnapshotUnavailable,SegmentedTransferFailureCode.PublishedSnapshotUnavailable); }

        await ValidatePublishedSnapshot(snapshot,itemid,cancel).ConfigureAwait(false);

        return snapshot;
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="MapFailureCode"]/*'/>*/
    private static SegmentedTransferFailureCode MapFailureCode(String message)
    {
        if(String.Equals(message,InvalidObjectHash,StringComparison.Ordinal))            { return SegmentedTransferFailureCode.InvalidObjectHash; }
        if(String.Equals(message,InvalidPublishedSnapshot,StringComparison.Ordinal))     { return SegmentedTransferFailureCode.InvalidPublishedSnapshot; }
        if(String.Equals(message,InvalidSegmentHash,StringComparison.Ordinal))           { return SegmentedTransferFailureCode.InvalidSegmentHash; }
        if(String.Equals(message,InvalidSegmentLength,StringComparison.Ordinal))         { return SegmentedTransferFailureCode.InvalidSegmentLength; }
        if(String.Equals(message,InvalidSessionState,StringComparison.Ordinal))          { return SegmentedTransferFailureCode.InvalidSessionState; }
        if(String.Equals(message,InvalidStateVersion,StringComparison.Ordinal))          { return SegmentedTransferFailureCode.InvalidStateVersion; }
        if(String.Equals(message,LoadSessionFailed,StringComparison.Ordinal))            { return SegmentedTransferFailureCode.LoadSessionFailed; }
        if(String.Equals(message,NotFullyRealized,StringComparison.Ordinal))             { return SegmentedTransferFailureCode.NotFullyRealized; }
        if(String.Equals(message,OpenFailed,StringComparison.Ordinal))                   { return SegmentedTransferFailureCode.OpenFailed; }
        if(String.Equals(message,PublishedSnapshotUnavailable,StringComparison.Ordinal)) { return SegmentedTransferFailureCode.PublishedSnapshotUnavailable; }
        if(String.Equals(message,RangeUnavailable,StringComparison.Ordinal))             { return SegmentedTransferFailureCode.RangeUnavailable; }
        if(String.Equals(message,ReOpenFailed,StringComparison.Ordinal))                 { return SegmentedTransferFailureCode.ReOpenFailed; }
        if(String.Equals(message,RemoveFailed,StringComparison.Ordinal))                 { return SegmentedTransferFailureCode.RemoveFailed; }
        if(String.Equals(message,SegmentUploadFailed,StringComparison.Ordinal))          { return SegmentedTransferFailureCode.SegmentUploadFailed; }
        if(String.Equals(message,SessionNotFound,StringComparison.Ordinal))              { return SegmentedTransferFailureCode.SessionNotFound; }
        if(String.Equals(message,StreamHashMismatch,StringComparison.Ordinal))           { return SegmentedTransferFailureCode.StreamHashMismatch; }
        if(String.Equals(message,TransferReadFailed,StringComparison.Ordinal))           { return SegmentedTransferFailureCode.TransferReadFailed; }

        return SegmentedTransferFailureCode.InvalidArgument;
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateAbortRequest"]/*'/>*/
    private static void ValidateAbortRequest(AbortTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateCommitRequest"]/*'/>*/
    private static void ValidateCommitRequest(CommitUploadTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.ExpectedStateVersion < 0) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateRemoveRequest"]/*'/>*/
    private static void ValidateRemoveRequest(RemoveTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateGetTransferSegmentRequest"]/*'/>*/
    private static void ValidateGetTransferSegmentRequest(GetTransferSegmentRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(CreateRange(request.Offset,request.Length).Validate() is false || request.Length <= 0) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateHashBytes"]/*'/>*/
    private static void ValidateHash(Byte[] actual , Byte[] expected , String message)
    {
        if(actual.AsSpan().SequenceEqual(expected) is false) { throw new OperationFailedException(message,MapFailureCode(message)); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateHashPayloadAsync"]/*'/>*/
    private static async Task ValidateHash(Byte[] expectedhash , Stream payload , String message , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(payload);

        if(payload.CanSeek) { payload.Position = 0; }

        Byte[] actual = await SHA512.HashDataAsync(payload,cancel).ConfigureAwait(false);

        if(payload.CanSeek) { payload.Position = 0; }

        if(actual.AsSpan().SequenceEqual(expectedhash) is false) { throw new OperationFailedException(message,MapFailureCode(message)); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateHashPayloadStreamAsync"]/*'/>*/
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

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateManifestHashes"]/*'/>*/
    private static void ValidateManifestHashes(DataItemTransferManifest manifest , Byte[] objecthash , Byte[] streamhash , Int64 streamlength)
    {
        ValidateHash(objecthash,manifest.ObjectSHA512,InvalidObjectHash);

        ValidateHash(streamhash,manifest.StreamSHA512,StreamHashMismatch);

        if(manifest.StreamLength != streamlength) { throw new OperationFailedException(InvalidArgument,SegmentedTransferFailureCode.InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateManifestIdentity"]/*'/>*/
    private static void ValidateManifestIdentity(DataItemTransferManifest manifest , Guid sessionid , Guid itemid)
    {
        if(manifest.SessionID != sessionid || manifest.ItemID != itemid) { throw new OperationFailedException(IdentityMismatch,SegmentedTransferFailureCode.IdentityMismatch); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateOpenGetRequest"]/*'/>*/
    private static void ValidateOpenGetRequest(OpenGetTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(request.SourceSessionID.HasValue && request.SourceSessionID.Value == Guid.Empty) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateOpenRequest"]/*'/>*/
    private static async Task ValidateOpenRequest(OpenUploadTransferRequest request , CancellationToken cancel)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(request.StreamLength < 0 || request.ObjectPayload is null) { throw new ArgumentException(InvalidArgument); }

        if(request.RequestedSegmentSizePolicy is not null && request.RequestedSegmentSizePolicy.Validate() is false) { throw new ArgumentException(InvalidArgument); }

        if(request.ObjectSHA512 is null || request.ObjectSHA512.Length != 64) { throw new ArgumentException(InvalidArgument); }

        if(request.StreamSHA512 is null || (request.StreamLength == 0 ? request.StreamSHA512.Length != 0 : request.StreamSHA512.Length != 64)) { throw new ArgumentException(InvalidArgument); }

        Byte[] objectPayloadHash = request.ObjectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(request.ObjectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(request.ObjectPayload);

        if(objectPayloadHash.AsSpan().SequenceEqual(request.ObjectSHA512) is false) { throw new OperationFailedException(InvalidObjectHash,SegmentedTransferFailureCode.InvalidObjectHash); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidatePublishedSnapshot"]/*'/>*/
    private static async Task ValidatePublishedSnapshot(PublishedTransferSnapshot snapshot , Guid itemId , CancellationToken cancel)
    {
        if(snapshot.ItemId != itemId || snapshot.ObjectPayload is null || snapshot.ObjectSHA512 is null || snapshot.ObjectSHA512.Length != 64 || snapshot.StreamSHA512 is null || snapshot.StreamLength < 0)
        {
            throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot);
        }

        if((snapshot.StreamLength == 0 && snapshot.StreamSHA512.Length != 0) || (snapshot.StreamLength > 0 && snapshot.StreamSHA512.Length != 64))
        {
            throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot);
        }

        Byte[] objectPayloadHash = snapshot.ObjectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(snapshot.ObjectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(snapshot.ObjectPayload);

        if(objectPayloadHash.AsSpan().SequenceEqual(snapshot.ObjectSHA512) is false) { throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot); }

        if(snapshot.RealizedRanges.Any(_ => _.Validate() is false || _.Length <= 0 || _.EndOffsetExclusive > snapshot.StreamLength)) { throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateReOpenGetRequest"]/*'/>*/
    private static void ValidateReOpenGetRequest(ReOpenGetTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(request.StreamSHA512 is null || (request.StreamSHA512.Length != 0 && request.StreamSHA512.Length != 64)) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateReOpenRequest"]/*'/>*/
    private static void ValidateReOpenRequest(ReOpenUploadTransferRequest request)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.StreamLength < 0) { throw new ArgumentException(InvalidArgument); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateSegmentHash"]/*'/>*/
    private static Task ValidateSegmentHash(Byte[] expectedhash , Stream payload , Int64 expectedlength , CancellationToken cancel)
    {
        return ValidateHash(expectedhash,payload,expectedlength,InvalidSegmentHash,cancel);
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateSegmentLengthAgainstPolicy"]/*'/>*/
    private void ValidateSegmentLengthAgainstPolicy(DataItemTransferManifest manifest , Int64 length , Int64 offset)
    {
        DataItemTransferSegmentSizePolicy policy = ClampPolicy(manifest.SegmentSizePolicy,this.Options.DefaultSegmentSizePolicy);

        if(policy.Validate() is false) { return; }

        Boolean isFinalSegment = offset >= 0 && length >= 0 && offset <= manifest.StreamLength && offset + length == manifest.StreamLength;

        if(this.Options.EnforceMaximumSegmentBytes && length > policy.MaxSegmentBytes) { throw new OperationFailedException(InvalidSegmentLength,SegmentedTransferFailureCode.InvalidSegmentLength); }

        if(this.Options.EnforceMinimumSegmentBytes && isFinalSegment is false && length < policy.MinSegmentBytes) { throw new OperationFailedException(InvalidSegmentLength,SegmentedTransferFailureCode.InvalidSegmentLength); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateSessionReadable"]/*'/>*/
    private static void ValidateSessionReadable(DataItemTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidateSessionWritable"]/*'/>*/
    private static void ValidateSessionWritable(DataItemTransferManifest manifest)
    {
        if(manifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Committed or DataItemTransferStatus.Committing or DataItemTransferStatus.Faulted) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }
    }

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/method[@name="ValidatePutTransferSegmentRequest"]/*'/>*/
    private static void ValidatePutTransferSegmentRequest(PutTransferSegmentRequest request , Stream payload)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        if(payload is null || payload.CanRead is false) { throw new ArgumentException(InvalidArgument); }

        if(CreateRange(request.Offset,request.Length).Validate() is false || request.Length <= 0 || request.ExpectedStateVersion < 0) { throw new ArgumentException(InvalidArgument); }
    }
}
